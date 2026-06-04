using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IServices
{
    public interface IEmployeeService
    {

        Task<(bool success, IEnumerable<Employee> employees)> GetAllEmployeeAsync(int page = 1, int pageSize = 5);
        Task<(bool success, Employee? employee)> GetEmployeeAsync(string id);
        Task<(bool success, string message)> AddEmployeeAsync(Employee employee);
        Task<(bool success, string message)> UpdateEmployeeAsync(Employee employee);
        Task<(bool success, string message)> DeleteEmployeeAsync(string id);

   
        Task<IEnumerable<Qualification>> GetQualificationsAsync();


        Task<IEnumerable<Vacation>> GetVacationsAsync(string empId);
        Task<(bool success, string message)> AddVacationAsync(Vacation vacation);
        Task<(bool success, string message)> DeleteVacationAsync(int id);

        
        Task<int> GetTotalEmployeesCountAsync();
    }
}
