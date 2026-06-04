using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class EmployeeService(IUnitOfWork unitOfWork) : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<(bool success, IEnumerable<Employee> employees)> GetAllEmployeeAsync(int page = 1, int pageSize = 5)
        {
            try
            {
                var skip = (page - 1) * pageSize;

                var employees = await _unitOfWork.EmployeeRepository.GetAsync(
                    expression: null,
                    include: q => q.Include(e => e.Vacations),
                    tracked: false,
                    skip: skip,
                    take: pageSize,
                    orderBy: q => q.OrderBy(e => e.Id)
                );

                return (true, employees);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return (false, new List<Employee>());
            }
        }

        public async Task<(bool success, Employee? employee)> GetEmployeeAsync(string id)
        {
            try
            {
                var employee = await _unitOfWork.EmployeeRepository.GetOneAsync(
                    expression: e => e.Id == id,
                    include: q => q.Include(e => e.Vacations).Include(e => e.Qualification),
                    tracked: false
                );

                return (true, employee);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return (false, null);
            }
        }

        public async Task<(bool success, string message)> AddEmployeeAsync(Employee employee)
        {
            try
            {
                var res = _unitOfWork.EmployeeRepository.GetQueryable(e => e.Id == employee.Id);
                if (res.Any() )
                {
                    return (false, "الرقم مستخدم مسبقاً!");
                }

                var res2 = _unitOfWork.EmployeeRepository.GetQueryable(e => e.Name == employee.Name);
                if (res2.Any())
                {
                    return (false, "الاسم مستخدم مسبقاً!");
                }

                await _unitOfWork.EmployeeRepository.CreateAsync(employee);
                return (true, "تم الحفظ بنجاح!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return (false, "حدث خطأ أثناء الحفظ!");
            }
        }

        public async Task<(bool success, string message)> UpdateEmployeeAsync(Employee employee)
        {
            try
            {
                var res =  _unitOfWork.EmployeeRepository.GetQueryable(e => e.Name == employee.Name && e.Id != employee.Id);
                if (res.Any())
                {
                    return (false, "الرقم او الاسم مستخدم مسبقاً!");
                }

                await _unitOfWork.EmployeeRepository.EditAsync(employee);
                return (true, "تم التعديل بنجاح!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return (false, "حدث خطأ أثناء التعديل!");
            }
        }

        public async Task<(bool success, string message)> DeleteEmployeeAsync(string id)
        {
            try
            {
                var employee = await _unitOfWork.EmployeeRepository.GetOneAsync(
                    expression: e => e.Id == id,
                    include: q => q.Include(e => e.Vacations)
                );

                if (employee == null)
                {
                    return (false, "الموظف غير موجود!");
                }

                
                if (employee.Vacations != null && employee.Vacations.Any())
                {
                    await _unitOfWork.VacationRepository.DeleteAllAsync(employee.Vacations);
                }

                await _unitOfWork.EmployeeRepository.DeleteAsync(employee);
                return (true, "تم الحذف بنجاح!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return (false, "حدث خطأ أثناء الحذف!");
            }
        }

        // ==================== المؤهلات ====================

        public async Task<IEnumerable<Qualification>> GetQualificationsAsync()
        {
            try
            {
                return await _unitOfWork.QualificationRepository.GetAsync(tracked: false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<Qualification>();
            }
        }

        // ==================== الإجازات ====================

        public async Task<IEnumerable<Vacation>> GetVacationsAsync(string empId)
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
                // التحقق من المدة (1-30 يوم)
                if (vacation.Duration < 1 || vacation.Duration > 30)
                {
                    return (false, "المدة يجب أن تكون بين 1 و 30 يوم!");
                }

                var newStart = vacation.StartDate;
                var newEnd = vacation.StartDate.AddDays(vacation.Duration - 1);

                // جلب إجازات الموظف الحالية
                var existingVacations = await _unitOfWork.VacationRepository.GetAsync(
                    expression: v => v.EmployeeId == vacation.EmployeeId
                );

                // التحقق من عدم التداخل (Requirement 6-i)
                foreach (var v in existingVacations)
                {
                    var vStart = v.StartDate;
                    var vEnd = v.StartDate.AddDays(v.Duration - 1);

                    if (newStart <= vEnd && newEnd >= vStart)
                    {
                        return (false, "لا يمكن تسجيل إجازتين في نفس الفترة!");
                    }
                }

                // التحقق من الحد السنوي 30 يوم (Requirement 6-ii)
                var currentYear = vacation.StartDate.Year;
                var yearTotal = existingVacations
                    .Where(v => v.StartDate.Year == currentYear)
                    .Sum(v => v.Duration);

                if (yearTotal + vacation.Duration > 30)
                {
                    return (false, $"لا يمكن تجاوز 30 يوم إجازة سنوياً! (المسجل: {yearTotal} يوم)");
                }

                // حفظ الإجازة
                await _unitOfWork.VacationRepository.CreateAsync(vacation);

                // تحديث إجمالي الإجازات (Requirement 7)
                var employee = await _unitOfWork.EmployeeRepository.GetOneAsync(
                    expression: e => e.Id == vacation.EmployeeId
                );

                if (employee != null)
                {
                    var totalDays = existingVacations.Sum(v => v.Duration) + vacation.Duration;
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

                // تحديث إجمالي الإجازات بعد الحذف
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



        public async Task<int> GetTotalEmployeesCountAsync()
        {
            try
            {
                return await _unitOfWork.EmployeeRepository.CountAsync();
            }
            catch
            {
                return 0;
            }
        }
    }

}

