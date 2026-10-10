using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CalendarApp.Application.Abstraction;
using CalendarApp.Application.Helpers;
using CalendarApp.Domain.Entities;
using CalendarApp.Application.Models;
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

        public IActionResult Index(string view = "list", int year = 0, int month = 0, DateTime? date = null)
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
                var displayDate = date ?? ((year > 0 && month > 0) ? new DateTime(year, month, 1) : DateTime.Now);

                // The 42-day grid can start before the month — expand over its full span
                // (weeks start Monday, same as the weekly view)
                var firstDayOfMonth = new DateTime(displayDate.Year, displayDate.Month, 1);
                var firstDayOfWeek = firstDayOfMonth.AddDays(-(((int)firstDayOfMonth.DayOfWeek + 6) % 7));
                var displayEvents = RecurrenceExpander.Expand(userEvents, firstDayOfWeek, firstDayOfWeek.AddDays(42));

                var monthlyViewModel = BuildMonthlyCalendar(displayEvents, displayDate);
                monthlyViewModel.CurrentView = "monthly";
                ViewBag.UserEvents = userEvents;
                return View("Index", monthlyViewModel);
            }

            // Daily / weekly time-grid views (shared lane layout)
            if (view == "daily" || view == "weekly")
            {
                var anchor = (date ?? DateTime.Now).Date;
                List<DateTime> gridDays;
                if (view == "daily")
                {
                    gridDays = new List<DateTime> { anchor };
                }
                else
                {
                    // week starts Monday
                    var weekStart = anchor.AddDays(-(((int)anchor.DayOfWeek + 6) % 7));
                    gridDays = Enumerable.Range(0, 7).Select(i => weekStart.AddDays(i)).ToList();
                }

                var displayEvents = RecurrenceExpander.Expand(userEvents, gridDays[0], gridDays[^1].Date.AddDays(1));

                var gridViewModel = BuildTimeGrid(displayEvents, gridDays);
                gridViewModel.CurrentView = view;
                ViewBag.UserEvents = userEvents;
                return View("Index", gridViewModel);
            }

            // Default list view — expand recurring occurrences over a bounded window
            // (unbounded rules would otherwise grow the list forever)
            var listStart = DateTime.Today.AddDays(-30);
            var listEnd = DateTime.Today.AddMonths(6);
            var listEvents = RecurrenceExpander.Expand(userEvents, listStart, listEnd)
                .OrderBy(e => e.StartTime)
                .ToList();

            var listViewModel = new MonthlyCalendarViewModel
            {
                CurrentDate = DateTime.Now,
                DisplayMonth = DateTime.Now,
                CurrentView = "list",
                Days = new List<CalendarDayViewModel>()
            };
            ViewBag.UserEvents = listEvents;
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

            // Get the first day of the month and the first day of the calendar week (weeks start Monday)
            var firstDayOfMonth = new DateTime(displayDate.Year, displayDate.Month, 1);
            var firstDayOfWeek = firstDayOfMonth.AddDays(-(((int)firstDayOfMonth.DayOfWeek + 6) % 7));

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

        private MonthlyCalendarViewModel BuildTimeGrid(List<Event> events, List<DateTime> days)
        {
            var today = DateTime.Today;
            return new MonthlyCalendarViewModel
            {
                CurrentDate = days[0],
                DisplayMonth = days[0],
                Days = days.Select(d => new CalendarDayViewModel
                {
                    Date = d,
                    IsCurrentMonth = true,
                    IsToday = d.Date == today,
                    Events = events
                        .Where(e => e.StartTime.Date <= d && e.EndTime.Date >= d)
                        .OrderBy(e => e.StartTime)
                        .ToList()
                }).ToList()
            };
        }

        public IActionResult Details(int id, DateTime? occurrenceDate, string returnView = "list", DateTime? returnDate = null)
        {
            ViewBag.ReturnView = returnView;
            ViewBag.ReturnDate = returnDate;

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

            var myAttendance = @event.EventAttendees.FirstOrDefault(ea => ea.UserId == userId);
            ViewBag.CanDecline = myAttendance != null && myAttendance.Role != AttendeeRole.Owner && myAttendance.Status != AttendeeStatus.Declined;
            ViewBag.IsDeclined = myAttendance?.Status == AttendeeStatus.Declined;
            ViewBag.Users = _eventAppService.GetUsersByIds(@event.EventAttendees.Select(ea => ea.UserId).Append(@event.OwnerId));

            // Recurring series: resolve the requested occurrence (exception overrides applied)
            if (@event.IsRecurring && occurrenceDate.HasValue)
            {
                var target = occurrenceDate.Value.Date;
                var occurrences = RecurrenceExpander.Expand(@event, target, target.AddDays(1));
                var occurrence = occurrences.FirstOrDefault(o => o.StartTime.Date == target) ?? occurrences.FirstOrDefault();
                if (occurrence != null)
                {
                    ViewBag.IsOccurrence = true;
                    return View(occurrence);
                }
                ViewBag.OccurrenceMissing = true;
            }

            return View(@event);
        }

        [HttpPost]
        public async Task<IActionResult> Decline(int id, string returnView = "list", DateTime? returnDate = null)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return RedirectToAction("Login", "Account");
            }

            if (await _eventAppService.DeclineEventAsync(id, userId))
            {
                TempData["Success"] = "You declined this event.";
            }
            else
            {
                TempData["Error"] = "This event cannot be declined.";
            }

            return RedirectToAction("Details", new { id, returnView, returnDate = returnDate?.ToString("yyyy-MM-dd") });
        }

        [HttpGet]
        public IActionResult Create(DateTime? date = null, bool allDay = false, string view = "list")
        {
            var startTime = date ?? DateTime.Now;
            var viewModel = new EventCreateViewModel
            {
                StartTime = startTime,
                EndTime = startTime.AddHours(1),
                AllDay = allDay,
                Color = "#3788d8",
                ReturnView = view,
                ReturnDate = date ?? DateTime.Now
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
                RecurrenceRule = viewModel.IsRecurring ? viewModel.RecurrenceRule : null,
                RecurrenceEnd = viewModel.IsRecurring ? viewModel.RecurrenceEnd : null,
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

                    // Return the user to the calendar view they came from
                    var returnView = viewModel.ReturnView;
                    if (returnView != "daily" && returnView != "weekly" && returnView != "monthly")
                    {
                        returnView = "list";
                    }
                    return RedirectToAction("Index", new { view = returnView, date = viewModel.ReturnDate });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error creating event: " + ex.Message);
                }
            }

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Edit(int id, string? scope, DateTime? occurrenceDate, string returnView = "details", DateTime? returnDate = null)
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

            ViewBag.Users = _eventAppService.GetUsersByIds(@event.EventAttendees.Select(ea => ea.UserId).Append(@event.OwnerId));

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
                CurrentAttendees = currentAttendees,
                ReturnView = returnView,
                ReturnDate = returnDate ?? @event.StartTime
            };

            // Recurring series opened from an occurrence link: prefill the single
            // occurrence (exception overrides applied), defaulting to "this" scope
            if (@event.IsRecurring && occurrenceDate.HasValue)
            {
                var target = occurrenceDate.Value.Date;
                var occurrences = RecurrenceExpander.Expand(@event, target, target.AddDays(1));
                var occurrence = occurrences.FirstOrDefault(o => o.StartTime.Date == target) ?? occurrences.FirstOrDefault();
                if (occurrence != null)
                {
                    viewModel.Scope = string.Equals(scope, "all", StringComparison.OrdinalIgnoreCase) ? "all" : "this";
                    viewModel.OccurrenceDate = occurrence.StartTime.Date;
                    viewModel.Title = occurrence.Title;
                    viewModel.Description = occurrence.Description;
                    viewModel.StartTime = occurrence.StartTime;
                    viewModel.EndTime = occurrence.EndTime;
                    viewModel.AllDay = occurrence.AllDay;
                    viewModel.Location = occurrence.Location;
                    viewModel.ReminderTime = occurrence.ReminderTime;
                    viewModel.Color = occurrence.Color;
                    viewModel.Status = occurrence.Status;
                }
            }

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

            // Update event from view model (Owner is taken from the stored event, never from the form)
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
                RecurrenceRule = viewModel.IsRecurring ? viewModel.RecurrenceRule : null,
                RecurrenceEnd = viewModel.IsRecurring ? viewModel.RecurrenceEnd : null,
                OwnerId = existingEvent.OwnerId,
                ReminderTime = viewModel.ReminderTime,
                Color = viewModel.Color,
                Status = viewModel.Status
            };

            if (ModelState.IsValid)
            {
                try
                {
                    // Per-occurrence edit: store an exception row, leave the series master untouched
                    var isOccurrenceEdit = existingEvent.IsRecurring
                        && viewModel.Scope == "this"
                        && viewModel.OccurrenceDate.HasValue;

                    if (isOccurrenceEdit)
                    {
                        _eventAppService.UpsertOccurrenceException(new EventOccurrenceException
                        {
                            MasterEventId = existingEvent.Id,
                            OriginalDate = viewModel.OccurrenceDate.Value.Date,
                            IsCancelled = false,
                            Title = viewModel.Title,
                            Description = viewModel.Description,
                            StartTime = viewModel.StartTime,
                            EndTime = viewModel.EndTime,
                            AllDay = viewModel.AllDay,
                            Location = viewModel.Location,
                            ReminderTime = viewModel.ReminderTime,
                            Color = viewModel.Color,
                            Status = viewModel.Status,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        });
                        TempData["Success"] = "Occurrence saved!";
                    }
                    else
                    {
                        _eventAppService.Update(@event);
                        TempData["Success"] = "Event saved!";
                    }

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

                    TempData["Success"] = TempData["Success"] ?? "Event saved!";

                    // Return the user to the view they came from
                    var returnView = viewModel.ReturnView;
                    if (returnView != "daily" && returnView != "weekly" && returnView != "monthly"
                        && returnView != "list" && returnView != "details")
                    {
                        returnView = "details";
                    }
                    if (returnView == "details")
                    {
                        return RedirectToAction("Details", new { id = viewModel.Id });
                    }
                    return RedirectToAction("Index", new { view = returnView, date = viewModel.ReturnDate });
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
            ViewBag.Users = _eventAppService.GetUsersByIds(existingEvent.EventAttendees.Select(ea => ea.UserId).Append(existingEvent.OwnerId));

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Delete(int id, string? scope, DateTime? occurrenceDate)
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
                    // Delete only one occurrence of a recurring series
                    if (existingEvent.IsRecurring
                        && string.Equals(scope, "this", StringComparison.OrdinalIgnoreCase)
                        && occurrenceDate.HasValue)
                    {
                        _eventAppService.UpsertOccurrenceException(new EventOccurrenceException
                        {
                            MasterEventId = existingEvent.Id,
                            OriginalDate = occurrenceDate.Value.Date,
                            IsCancelled = true,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        });
                        TempData["Success"] = "Occurrence deleted.";
                        return RedirectToAction(nameof(Index));
                    }

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

        [HttpGet]
        public async Task<IActionResult> SearchUsers(string term)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Trim().Length < 3)
            {
                return Json(Enumerable.Empty<object>());
            }

            var users = await _eventAppService.SearchUsersAsync(term.Trim());
            return Json(users.Select(u => new { u.Id, u.UserName, u.Email }));
        }
    }
}