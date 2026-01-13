using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CalendarApp.Application.Abstraction;
using CalendarApp.Domain.Entities;
using CalendarApp.Web.Models;
using System.Security.Claims;

namespace CalendarApp.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class EventController : Controller
    {
        private readonly IEventAppService _eventAppService;
        private readonly IUserAppService _userAppService;

        public EventController(IEventAppService eventAppService, IUserAppService userAppService)
        {
            _eventAppService = eventAppService;
            _userAppService = userAppService;
        }

        public IActionResult Select()
        {
            var events = _eventAppService.SelectAll();
            return View(events);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var allUsers = await _userAppService.GetAllUsersAsync();
            var currentAdminId = GetCurrentUserId();
            
            var viewModel = new AdminEventCreateViewModel
            {
                AllUsers = allUsers.ToList(),
                FilteredUsers = allUsers.ToList(),
                OwnerId = currentAdminId,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                Color = "#3788d8"
            };
            
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AdminEventCreateViewModel viewModel)
        {
            // Re-populate users for view return
            var allUsers = await _userAppService.GetAllUsersAsync();
            viewModel.AllUsers = allUsers.ToList();
            viewModel.FilteredUsers = FilterUsersBySearch(allUsers.ToList(), viewModel.SearchTerm);

            if (ModelState.IsValid)
            {
                var @event = new Event
                {
                    Title = viewModel.Title,
                    Description = viewModel.Description,
                    StartTime = viewModel.StartTime,
                    EndTime = viewModel.EndTime,
                    AllDay = viewModel.AllDay,
                    Location = viewModel.Location,
                    IsRecurring = viewModel.IsRecurring,
                    RecurrenceRule = viewModel.RecurrenceRule,
                    RecurrenceEnd = viewModel.RecurrenceEnd,
                    OwnerId = viewModel.OwnerId,
                    ReminderTime = viewModel.ReminderTime,
                    Color = viewModel.Color,
                    Status = viewModel.Status
                };

                try
                {
                    _eventAppService.Create(@event);

                    // Invite additional attendees (excluding owner)
                    if (viewModel.AttendeeIds.Any())
                    {
                        await _eventAppService.InviteUsersToEvent(@event.Id, viewModel.AttendeeIds);
                    }

                    var ownerName = allUsers.FirstOrDefault(u => u.Id == viewModel.OwnerId)?.UserName ?? "Unknown";
                    TempData["Success"] = $"Event created for {ownerName} with {viewModel.AttendeeIds.Count} additional attendees!";
                    return RedirectToAction(nameof(Select));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error creating event: " + ex.Message);
                }
            }

            return View(viewModel);
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

        private IList<UserWithRoleDto> FilterUsersBySearch(IList<UserWithRoleDto> users, string? searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm) || searchTerm.Length < 2)
                return users;

            return users.Where(u => 
                u.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        public IActionResult Details(int id)
        {
            var @event = _eventAppService.GetById(id);
            if (@event == null)
            {
                return NotFound();
            }
            return View(@event);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var @event = _eventAppService.GetById(id);
            if (@event == null)
            {
                return NotFound();
            }
            
            return View(@event);
        }

        [HttpPost]
        public IActionResult Edit(Event @event)
        {
            // Remove Owner validation error since we're using OwnerId
            ModelState.Remove("Owner");
            
            if (ModelState.IsValid)
            {
                try
                {
                    _eventAppService.Update(@event);
                    TempData["Success"] = "Event updated successfully!";
                    return RedirectToAction(nameof(Select));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error updating event: " + ex.Message);
                }
            }
            return View(@event);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            bool deleted = _eventAppService.Delete(id);
            if (deleted)
            {
                return RedirectToAction(nameof(Select));
            }
            return NotFound();
        }
    }
}