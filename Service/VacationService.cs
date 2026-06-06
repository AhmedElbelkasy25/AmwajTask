using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class VacationService (IUnitOfWork unitOfWork): IVacationService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<IEnumerable<Vacation>> GetVacationsAsync(int empId)
        {
            try
            {
                return await _unitOfWork.VacationRepository.GetAsync(
                    expression: v => v.EmployeeId == empId,
                    orderBy: q => q.OrderByDescending(v => v.StartDate),
                    tracked: false
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<Vacation>();
            }
        }

        public async Task<(bool success, string message)> AddVacationAsync(Vacation vacation)
        {
            try
            {

                if (vacation.Duration < 1 || vacation.Duration > 30)
                {
                    return (false, "المدة يجب أن تكون بين 1 و 30 يوم!");
                }

                if (vacation.StartDate < (DateOnly.FromDateTime(DateTime.Now)))
                {
                    return (false, " لا يمكنك اختيار تاريخ اقل من تاريخ اليوم الحالي!");
                }

                var newStart = vacation.StartDate;

                var newEnd = vacation.StartDate.AddDays(vacation.Duration - 1);


                var existingVacations = _unitOfWork.VacationRepository.GetQueryable(
                    expression: v => v.EmployeeId == vacation.EmployeeId
                );

                var currentYear = vacation.StartDate.Year;
                var yearTotal = existingVacations
                    .Where(v => v.StartDate.Year == currentYear)
                    .Sum(v => v.Duration);

                if (yearTotal + vacation.Duration > 30)
                {
                    return (false, $"لا يمكن تجاوز 30 يوم إجازة سنوياً! (المسجل: {yearTotal} يوم)");
                }

                foreach (var v in existingVacations)
                {
                    var vStart = v.StartDate;
                    var vEnd = v.StartDate.AddDays(v.Duration - 1);

                    if (newStart <= vEnd && newEnd >= vStart)
                    {
                        return (false, "لا يمكن تسجيل إجازتين في نفس الفترة!");
                    }
                }





                await _unitOfWork.VacationRepository.CreateAsync(vacation);


                var employee = await _unitOfWork.EmployeeRepository.GetOneAsync(
                    expression: e => e.Id == vacation.EmployeeId
                );

                if (employee != null)
                {
                    var totalDays = existingVacations.Sum(v => v.Duration);
                    employee.TotalVacationDays = totalDays;
                    await _unitOfWork.EmployeeRepository.EditAsync(employee);
                }

                return (true, "تم إضافة الإجازة بنجاح!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return (false, "حدث خطأ أثناء إضافة الإجازة!");
            }
        }

        public async Task<(bool success, string message)> DeleteVacationAsync(int id)
        {
            try
            {
                var vacation = await _unitOfWork.VacationRepository.GetOneAsync(
                    expression: v => v.Id == id
                );

                if (vacation == null)
                {
                    return (false, "الإجازة غير موجودة!");
                }

                var empId = vacation.EmployeeId;

                await _unitOfWork.VacationRepository.DeleteAsync(vacation);


                var remainingVacations = await _unitOfWork.VacationRepository.GetAsync(
                    expression: v => v.EmployeeId == empId
                );

                var employee = await _unitOfWork.EmployeeRepository.GetOneAsync(
                    expression: e => e.Id == empId
                );

                if (employee != null)
                {
                    employee.TotalVacationDays = remainingVacations.Sum(v => v.Duration);
                    await _unitOfWork.EmployeeRepository.EditAsync(employee);
                }

                return (true, "تم حذف الإجازة بنجاح!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return (false, "حدث خطأ أثناء حذف الإجازة!");
            }
        }
    }
}
