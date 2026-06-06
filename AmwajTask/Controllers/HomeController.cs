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

        public HomeController(ILogger<HomeController> logger , IEmployeeService employeeService,
            IVacationService vacationService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            if (page <= 0)
            {
                page = 1;
            }
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
                NewEmployee =  new Employee() ,
            };

            return View(model);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
