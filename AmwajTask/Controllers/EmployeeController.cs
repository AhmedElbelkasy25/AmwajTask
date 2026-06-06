using Microsoft.AspNetCore.Mvc;
using Service.IServices;

namespace AmwajTask.Controllers
{
    public class EmployeeController : Controller
    {

        private readonly IEmployeeService _employeeService;
        private readonly IVacationService _vacationService;


        public EmployeeController(IEmployeeService employeeService, IVacationService vacationService)
        {
            _employeeService = employeeService;
            _vacationService = vacationService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(IndexVM vm)
        {
            ModelState.Remove("NewEmployee.Qualification");
            if (!ModelState.IsValid)
            {

                var (suc, emps) = await _employeeService.GetAllEmployeeAsync();
                vm.Empolyees = emps.ToList();
                vm.Qualifications = await _employeeService.GetQualificationsAsync();

                return View("~/Views/Home/Index.cshtml", vm);
            }
            var (success, message) = await _employeeService.AddEmployeeAsync(vm.NewEmployee);
            if (!success)
            {
                TempData["Error"] = message;

                var (suc, emps) = await _employeeService.GetAllEmployeeAsync();
                vm.Empolyees = emps.ToList();
                vm.Qualifications = await _employeeService.GetQualificationsAsync();
                
                return View("~/Views/Home/Index.cshtml", vm);
            }
            else TempData["Success"] = message;

            return RedirectToAction(nameof(Index), "Home" );
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Employee employee)
        {
            ModelState.Remove("Qualification");
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "بيانات غير صحيحة";
                return RedirectToAction(nameof(Index));
            }

            var (success, message) = await _employeeService.UpdateEmployeeAsync(employee);
            TempData[success ? "Success" : "Error"] = message;

            return RedirectToAction(nameof(Index), "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, message) = await _employeeService.DeleteEmployeeAsync(id);
            TempData[success ? "Success" : "Error"] = message;
            return RedirectToAction(nameof(Index), "Home");
        }

        public async Task<IActionResult> Print(int id)
        {
            var (success, emp) = await _employeeService.GetEmployeeAsync(id);
            var vacations = await _vacationService.GetVacationsAsync(id);

            var vm = new PrintVM
            {
                Employee = emp,
                Vacations = vacations.ToList()
            };

            return View(vm);
        }
    }
}
