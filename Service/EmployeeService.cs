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

        public async Task<(bool success, Employee? employee)> GetEmployeeAsync(int id)
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

        public async Task<(bool success, string message)> DeleteEmployeeAsync(int id)
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

