using Microsoft.AspNetCore.Mvc;
using Service;
using Service.IServices;

namespace AmwajTask.Controllers
{
    public class VacationController : Controller
    {
        private readonly IVacationService _vacationService;

        public VacationController(IVacationService vacationService)
        {
            _vacationService = vacationService;
        }

     
        public async Task<IActionResult> GetVacations(int empId)
        {
            var vacations = await _vacationService.GetVacationsAsync(empId);
            return Json(vacations);
        }

        [HttpPost]
        public async Task<IActionResult> AddVacation([FromBody] Vacation vacation)
        {
            var (success, message) = await _vacationService.AddVacationAsync(vacation);
            return Json(new { success, message });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteVacation(int id)
        {
            var (success, message) = await _vacationService.DeleteVacationAsync(id);
            return Json(new { success, message });
        }

        


    }
}
