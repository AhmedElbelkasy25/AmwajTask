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
        Task<(bool success, Employee? employee)> GetEmployeeAsync(int id);
        Task<(bool success, string message)> AddEmployeeAsync(Employee employee);
        Task<(bool success, string message)> UpdateEmployeeAsync(Employee employee);
        Task<(bool success, string message)> DeleteEmployeeAsync(int id);

   
        Task<IEnumerable<Qualification>> GetQualificationsAsync();


        
        Task<int> GetTotalEmployeesCountAsync();
    }
}
