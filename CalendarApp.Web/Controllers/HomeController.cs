using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CalendarApp.Web.Models;

namespace CalendarApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Check if user is logged in via Identity
            if (User.Identity?.IsAuthenticated == true)
            {
                // User is logged in, redirect based on role
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Select", "Event", new { area = "Admin" });
                }
                else
                {
                    return RedirectToAction("Index", "Events");
                }
            }

            // User not logged in, show home page
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
