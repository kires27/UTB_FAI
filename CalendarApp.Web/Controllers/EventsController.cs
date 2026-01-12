using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CalendarApp.Application.Abstraction;
using CalendarApp.Domain.Entities;
using CalendarApp.Web.Models;
using System.Security.Claims;

namespace CalendarApp.Web.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {
        private readonly IEventAppService _eventAppService;

        public EventsController(IEventAppService eventAppService)
        {
            _eventAppService = eventAppService;
        }

        public IActionResult Index(string view = "list", int year = 0, int month = 0)
        {
            // Get current user ID from Identity claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var username = User.Identity?.Name ?? "";
            
            // Get all events for the user (owner or attendee with accepted status)
            var allEvents = _eventAppService.SelectAll();
            var userEvents = allEvents.Where(e => e.OwnerId == userId || 
                e.EventAttendees.Any(ea => ea.UserId == userId && ea.Status == AttendeeStatus.Accepted))
                .OrderBy(e => e.StartTime)
                .ToList();

            ViewBag.Username = username;

            // If monthly view is requested
            if (view == "monthly")
            {
                // Default to current month if not specified
                var displayDate = (year > 0 && month > 0) ? new DateTime(year, month, 1) : DateTime.Now;
                var monthlyViewModel = BuildMonthlyCalendar(userEvents, displayDate);
                monthlyViewModel.CurrentView = "monthly";
                ViewBag.UserEvents = userEvents;
                return View("Index", monthlyViewModel);
            }

            // Default list view
            var listViewModel = new MonthlyCalendarViewModel
            {
                CurrentDate = DateTime.Now,
                DisplayMonth = DateTime.Now,
                CurrentView = "list",
                Days = new List<CalendarDayViewModel>()
            };
            ViewBag.UserEvents = userEvents;
            return View("Index", listViewModel);
        }

        private MonthlyCalendarViewModel BuildMonthlyCalendar(List<Event> events, DateTime displayDate)
        {
            var viewModel = new MonthlyCalendarViewModel
            {
                CurrentDate = DateTime.Now,
                DisplayMonth = displayDate,
                CurrentView = "monthly",
                Days = new List<CalendarDayViewModel>()
            };

            // Get the first day of the month and first day of the calendar week (Sunday)
            var firstDayOfMonth = new DateTime(displayDate.Year, displayDate.Month, 1);
            var firstDayOfWeek = firstDayOfMonth.AddDays(-(int)firstDayOfMonth.DayOfWeek);

            // Generate 6 weeks (42 days) to cover the entire month
            for (int week = 0; week < 6; week++)
            {
                for (int day = 0; day < 7; day++)
                {
                    var currentDate = firstDayOfWeek.AddDays(week * 7 + day);
                    var dayEvents = events.Where(e => 
                        (e.StartTime.Date <= currentDate.Date && e.EndTime.Date >= currentDate.Date))
                        .ToList();

                    viewModel.Days.Add(new CalendarDayViewModel
                    {
                        Date = currentDate,
                        IsCurrentMonth = currentDate.Month == displayDate.Month,
                        IsToday = currentDate.Date == DateTime.Today,
                        Events = dayEvents.OrderBy(e => e.StartTime).ToList()
                    });
                }
            }

            return viewModel;
        }

        public IActionResult Details(int id)
        {
            var @event = _eventAppService.GetById(id);
            if (@event == null)
            {
                return NotFound();
            }

            // Get current user ID from Identity claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return RedirectToAction("Login", "Account");
            }
            
            // Check if user has access to this event
            if (@event.OwnerId != userId && !@event.EventAttendees.Any(ea => ea.UserId == userId))
            {
                return Forbid();
            }

            return View(@event);
        }

        [HttpGet]
        public IActionResult Create(DateTime? date = null)
        {
            var startTime = date ?? DateTime.Now;
            var viewModel = new EventCreateViewModel
            {
                StartTime = startTime,
                EndTime = startTime.AddHours(1),
                Color = "#3788d8"
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(EventCreateViewModel viewModel)
        {
            // Get current user ID from Identity claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Create event from view model
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
                OwnerId = userId,
                ReminderTime = viewModel.ReminderTime,
                Color = viewModel.Color,
                Status = viewModel.Status
            };

            if (ModelState.IsValid)
            {
                try
                {
                    _eventAppService.Create(@event);

                    // Invite users by email
                    if (!string.IsNullOrEmpty(viewModel.InviteEmails))
                    {
                        var emailList = viewModel.InviteEmails
                            .Split(',')
                            .Select(e => e.Trim())
                            .Where(e => !string.IsNullOrEmpty(e) && e.Contains("@"))
                            .ToList();

                        if (emailList.Any())
                        {
                            var usersToInvite = await _eventAppService.FindUsersByEmailsAsync(emailList);
                            var userIdsToInvite = usersToInvite.Select(u => u.Id).ToList();

                            if (userIdsToInvite.Any())
                            {
                                await _eventAppService.InviteUsersToEvent(@event.Id, userIdsToInvite);
                            }
                        }
                    }

                    TempData["Success"] = "Event saved!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error creating event: " + ex.Message);
                }
            }

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var @event = _eventAppService.GetById(id);
            if (@event == null)
            {
                return NotFound();
            }

            // Get current user ID from Identity claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Check if user owns this event
            if (@event.OwnerId != userId)
            {
                return Forbid();
            }

            // Get current attendees (excluding owner)
            var currentAttendees = @event.EventAttendees
                .Where(ea => ea.Role != AttendeeRole.Owner)
                .ToList();

            var viewModel = new EventEditViewModel
            {
                Id = @event.Id,
                Title = @event.Title,
                Description = @event.Description,
                StartTime = @event.StartTime,
                EndTime = @event.EndTime,
                AllDay = @event.AllDay,
                Location = @event.Location,
                IsRecurring = @event.IsRecurring,
                RecurrenceRule = @event.RecurrenceRule,
                RecurrenceEnd = @event.RecurrenceEnd,
                OwnerId = @event.OwnerId,
                ReminderTime = @event.ReminderTime,
                Color = @event.Color,
                Status = @event.Status,
                CurrentAttendees = currentAttendees
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EventEditViewModel viewModel)
        {
            // Get current user ID from Identity claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Get existing event to verify ownership
            var existingEvent = _eventAppService.GetById(viewModel.Id);
            if (existingEvent == null)
            {
                return NotFound();
            }

            // Check if user owns this event
            if (existingEvent.OwnerId != userId)
            {
                return Forbid();
            }

            // Update event from view model
            var @event = new Event
            {
                Id = viewModel.Id,
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

            if (ModelState.IsValid)
            {
                try
                {
                    _eventAppService.Update(@event);

                    // Remove selected attendees
                    if (viewModel.RemoveAttendeeIds.Any())
                    {
                        foreach (var attendeeId in viewModel.RemoveAttendeeIds)
                        {
                            await _eventAppService.RemoveAttendeeFromEvent(viewModel.Id, attendeeId);
                        }
                    }

                    // Invite users by email
                    if (!string.IsNullOrEmpty(viewModel.InviteEmails))
                    {
                        var emailList = viewModel.InviteEmails
                            .Split(',')
                            .Select(e => e.Trim())
                            .Where(e => !string.IsNullOrEmpty(e) && e.Contains("@"))
                            .ToList();

                        if (emailList.Any())
                        {
                            var usersToInvite = await _eventAppService.FindUsersByEmailsAsync(emailList);
                            var userIdsToInvite = usersToInvite.Select(u => u.Id).ToList();

                            if (userIdsToInvite.Any())
                            {
                                await _eventAppService.InviteUsersToEvent(viewModel.Id, userIdsToInvite);
                            }
                        }
                    }

                    TempData["Success"] = "Event saved!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error updating event: " + ex.Message);
                }
            }

            // Repopulate data on error
            viewModel.CurrentAttendees = existingEvent.EventAttendees
                .Where(ea => ea.Role != AttendeeRole.Owner)
                .ToList();

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            // Get current user ID from Identity claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Get existing event to verify ownership
            var existingEvent = _eventAppService.GetById(id);
            if (existingEvent == null)
            {
                return NotFound();
            }

            // Check if user owns this event
            if (existingEvent.OwnerId != userId)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    bool deleted = _eventAppService.Delete(id);
                    if (deleted)
                    {
                        TempData["Success"] = "Event deleted successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                    ModelState.AddModelError("", "Error deleting event");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error deleting event: " + ex.Message);
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}