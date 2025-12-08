using Microsoft.AspNetCore.Mvc;
using CalendarApp.Application.Abstraction;
using CalendarApp.Domain.Entities;
using CalendarApp.Web.Models;

namespace CalendarApp.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserAppService _userAppService;

        public AccountController(IUserAppService userAppService)
        {
            _userAppService = userAppService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.Username,
                    Email = model.Email,
                    FullName = model.FullName
                };

                var result = await _userAppService.CreateUserAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Simple cookie authentication
                    HttpContext.Session.SetString("UserId", user.Id.ToString());
                    HttpContext.Session.SetString("Username", user.UserName);
                    HttpContext.Session.SetString("Role", user.Role ?? "User");
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", result.ErrorMessage ?? "Registration failed");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userAppService.ValidateUserAsync(model.Username, model.Password);

                if (result.Succeeded)
                {
                    HttpContext.Session.SetString("UserId", result.UserId?.ToString() ?? "");
                    HttpContext.Session.SetString("Username", result.Username ?? "");
                    HttpContext.Session.SetString("Role", result.Role ?? "User");
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid login attempt");
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}