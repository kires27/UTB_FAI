using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CalendarApp.Application.Abstraction;
using CalendarApp.Domain.Entities;
using System.Security.Claims;

namespace CalendarApp.Web.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly INotificationAppService _notificationAppService;

        public NotificationsController(INotificationAppService notificationAppService)
        {
            _notificationAppService = notificationAppService;
        }

        // GET: Notifications
        public async Task<IActionResult> Index()
        {
            // Get current user ID from Identity claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var notifications = await _notificationAppService.GetUserNotificationsAsync(userId);
            ViewBag.Username = User.Identity?.Name ?? "";
            ViewBag.UnreadCount = await _notificationAppService.GetUnreadCountAsync(userId);

            return View(notifications);
        }

        // POST: Notifications/MarkAsRead/5
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            // Get current user ID from Identity claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var result = await _notificationAppService.MarkAsReadAsync(id);
            
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = result });
            }

            if (result)
            {
                TempData["Success"] = "Notification marked as read!";
            }
            else
            {
                TempData["Error"] = "Notification not found or already processed.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Notifications/AcceptInvitation/5
        [HttpPost]
        public async Task<IActionResult> AcceptInvitation(int id)
        {
            // Get current user ID from Identity claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var result = await _notificationAppService.AcceptInvitationAsync(id, userId);
            
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var unreadCount = await _notificationAppService.GetUnreadCountAsync(userId);
                return Json(new { success = result, unreadCount = unreadCount });
            }

            if (result)
            {
                TempData["Success"] = "Invitation accepted! Event added to your calendar.";
            }
            else
            {
                TempData["Error"] = "Invitation not found or already processed.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Notifications/DeclineInvitation/5
        [HttpPost]
        public async Task<IActionResult> DeclineInvitation(int id)
        {
            // Get current user ID from Identity claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var result = await _notificationAppService.DeclineInvitationAsync(id, userId);
            
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var unreadCount = await _notificationAppService.GetUnreadCountAsync(userId);
                return Json(new { success = result, unreadCount = unreadCount });
            }

            if (result)
            {
                TempData["Success"] = "Invitation declined.";
            }
            else
            {
                TempData["Error"] = "Invitation not found or already processed.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Notifications/GetUnreadCount (for AJAX calls)
        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            // Get current user ID from Identity claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Json(new { unreadCount = 0 });
            }

            var unreadCount = await _notificationAppService.GetUnreadCountAsync(userId);
            return Json(new { unreadCount = unreadCount });
        }
    }
}