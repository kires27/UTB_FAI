using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CalendarApp.Application.Abstraction;
using CalendarApp.Web.Models;
using System.Security.Claims;

namespace CalendarApp.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserManagementController : Controller
    {
        private readonly IUserAppService _userAppService;

        public UserManagementController(IUserAppService userAppService)
        {
            _userAppService = userAppService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userAppService.GetAllUsersAsync();
            var viewModel = new UserListViewModel { Users = users };
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new UserCreateViewModel();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _userAppService.CreateUserAsync(
                        viewModel.Username, 
                        viewModel.Email, 
                        viewModel.Password, 
                        viewModel.Role);

                    if (result)
                    {
                        TempData["Success"] = "User created successfully!";
                        return RedirectToAction(nameof(Index));
                    }

                    ModelState.AddModelError("", "Failed to create user. Please check the input and try again.");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error creating user: " + ex.Message);
                }
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Get current user ID to prevent self-deletion
                var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(currentUserIdClaim, out var currentUserId) && currentUserId == id)
                {
                    TempData["Error"] = "You cannot delete your own account.";
                    return RedirectToAction(nameof(Index));
                }

                var result = await _userAppService.DeleteUserAsync(id);
                if (result)
                {
                    TempData["Success"] = "User deleted successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to delete user.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error deleting user: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}