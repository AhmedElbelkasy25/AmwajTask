using AmwajTask.Models;
using Microsoft.AspNetCore.Mvc;
using Service.IServices;
using System.Diagnostics;

namespace AmwajTask.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmployeeService _employeeService;

        public HomeController(ILogger<HomeController> logger , IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var (success, emps) = await _employeeService.GetAllEmployeeAsync(page);
            var qualifications = await _employeeService.GetQualificationsAsync();

            var totalCount = await _employeeService.GetTotalEmployeesCountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / 5.0);

            IndexVM model = new IndexVM()
            {
                CurrentPage = page,
                Qualifications = qualifications,
                TotalPages = totalPages,
                Empolyees = emps.ToList(),
                NewEmployee = new Employee() ,
            };

            return View(model);
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

                return View("Index", vm);
            }
            var (success, message) = await _employeeService.AddEmployeeAsync(vm.NewEmployee);
            if (!success) TempData["Error"] = message;
            else TempData["Success"] = message;

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Employee employee)
        {
            var (success, message) = await _employeeService.UpdateEmployeeAsync(employee);
            if (!success) TempData["Error"] = message;
            else TempData["Success"] = message;

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var (success, message) = await _employeeService.DeleteEmployeeAsync(id);
            TempData[success ? "Success" : "Error"] = message;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetVacations(string empId)
        {
            var vacations = await _employeeService.GetVacationsAsync(empId);
            return Json(vacations);
        }

        [HttpPost]
        public async Task<IActionResult> AddVacation([FromBody] Vacation vacation)
        {
            var (success, message) = await _employeeService.AddVacationAsync(vacation);
            return Json(new { success, message });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteVacation(int id)
        {
            var (success, message) = await _employeeService.DeleteVacationAsync(id);
            return Json(new { success, message });
        }

        public async Task<IActionResult> Print(string id)
        {
            var (success, emp) = await _employeeService.GetEmployeeAsync(id);
            return View(emp);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
