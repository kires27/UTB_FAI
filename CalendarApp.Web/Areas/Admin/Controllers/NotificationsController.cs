using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CalendarApp.Application.Abstraction;

namespace CalendarApp.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class NotificationsController : Controller
    {
        private readonly INotificationAppService _notificationAppService;

        public NotificationsController(INotificationAppService notificationAppService)
        {
            _notificationAppService = notificationAppService;
        }

        public async Task<IActionResult> Index()
        {
            var notifications = await _notificationAppService.SelectAllForAdminAsync();
            return View(notifications);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _notificationAppService.DeleteAsync(id);
            TempData[deleted ? "Success" : "Error"] = deleted
                ? "Notification deleted."
                : "Notification not found.";
            return RedirectToAction(nameof(Index));
        }
    }
}
