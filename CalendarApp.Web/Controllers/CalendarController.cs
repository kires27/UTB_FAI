using Microsoft.AspNetCore.Mvc;
using CalendarApp.Application.Abstraction;
using CalendarApp.Domain.Entities;

namespace CalendarApp.Web.Controllers
{
    public class CalendarController : Controller
    {
        private readonly IEventAppService _eventAppService;

        public CalendarController(IEventAppService eventAppService)
        {
            _eventAppService = eventAppService;
        }

        public IActionResult Index(DateTime? date = null)
        {
            // Check if user is logged in via session
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdStr);
            var username = HttpContext.Session.GetString("Username") ?? "";
            
            // Get all events for the current month
            var allEvents = _eventAppService.SelectAll();
            var userEvents = allEvents.Where(e => e.OwnerId == userId || 
                e.EventAttendees.Any(ea => ea.UserId == userId)).ToList();
            
            var calendarViewModel = new CalendarViewModel
            {
                CurrentDate = date ?? DateTime.Now,
                Events = userEvents,
                UserId = userId
            };

            ViewBag.Username = username;
            return View(calendarViewModel);
        }

        public IActionResult Details(int id)
        {
            var @event = _eventAppService.GetById(id);
            if (@event == null)
            {
                return NotFound();
            }

            var userId = int.Parse(HttpContext.Session.GetString("UserId")!);
            
            // Check if user has access to this event
            if (@event.OwnerId != userId && !@event.EventAttendees.Any(ea => ea.UserId == userId))
            {
                return Forbid();
            }

            return View(@event);
        }
    }

    public class CalendarViewModel
    {
        public DateTime CurrentDate { get; set; }
        public List<Event> Events { get; set; } = new();
        public int UserId { get; set; }
        
        public DateTime FirstDayOfMonth => new DateTime(CurrentDate.Year, CurrentDate.Month, 1);
        public DateTime LastDayOfMonth => FirstDayOfMonth.AddMonths(1).AddDays(-1);
        public int DaysInMonth => DateTime.DaysInMonth(CurrentDate.Year, CurrentDate.Month);
        public int StartingDayOfWeek => (int)FirstDayOfMonth.DayOfWeek;
        
        public List<CalendarDay> CalendarDays
        {
            get
            {
                var days = new List<CalendarDay>();
                var currentDate = FirstDayOfMonth.AddDays(-StartingDayOfWeek);
                
                // Add days from previous month
                for (int i = 0; i < StartingDayOfWeek; i++)
                {
                    days.Add(new CalendarDay
                    {
                        Date = currentDate,
                        IsCurrentMonth = false,
                        Events = new List<Event>()
                    });
                    currentDate = currentDate.AddDays(1);
                }
                
                // Add days from current month
                for (int day = 1; day <= DaysInMonth; day++)
                {
                    var date = new DateTime(CurrentDate.Year, CurrentDate.Month, day);
                    days.Add(new CalendarDay
                    {
                        Date = date,
                        IsCurrentMonth = true,
                        Events = Events.Where(e => e.StartTime.Date == date).ToList()
                    });
                }
                
                // Add days from next month to complete the week
                while (days.Count % 7 != 0)
                {
                    days.Add(new CalendarDay
                    {
                        Date = currentDate,
                        IsCurrentMonth = false,
                        Events = new List<Event>()
                    });
                    currentDate = currentDate.AddDays(1);
                }
                
                return days;
            }
        }
    }

    public class CalendarDay
    {
        public DateTime Date { get; set; }
        public bool IsCurrentMonth { get; set; }
        public List<Event> Events { get; set; } = new();
        public bool IsToday => Date.Date == DateTime.Today;
        public bool IsWeekend => Date.DayOfWeek == DayOfWeek.Saturday || Date.DayOfWeek == DayOfWeek.Sunday;
    }
}