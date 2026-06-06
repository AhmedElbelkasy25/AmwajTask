using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IServices
{
    public interface IVacationService
    {
        Task<IEnumerable<Vacation>> GetVacationsAsync(int empId);
        Task<(bool success, string message)> AddVacationAsync(Vacation vacation);
        Task<(bool success, string message)> DeleteVacationAsync(int id);

    }
}
