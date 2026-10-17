using CalendarApp.Application.Abstraction;
using CalendarApp.Application.Models;
using CalendarApp.Domain.Entities;

namespace CalendarApp.Application.Implementation
{
    public class CalendarViewAppService : ICalendarViewAppService
    {
        public MonthlyCalendarViewModel BuildList()
        {
            return new MonthlyCalendarViewModel
            {
                CurrentDate = DateTime.Now,
                DisplayMonth = DateTime.Now,
                CurrentView = "list",
                Days = new List<CalendarDayViewModel>()
            };
        }

        public MonthlyCalendarViewModel BuildMonthlyCalendar(List<Event> events, DateTime displayDate)
        {
            var viewModel = new MonthlyCalendarViewModel
            {
                CurrentDate = DateTime.Now,
                DisplayMonth = displayDate,
                CurrentView = "monthly",
                Days = new List<CalendarDayViewModel>()
            };

            // Weeks start Monday
            var firstDayOfMonth = new DateTime(displayDate.Year, displayDate.Month, 1);
            var firstDayOfWeek = firstDayOfMonth.AddDays(-(((int)firstDayOfMonth.DayOfWeek + 6) % 7));

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

        public MonthlyCalendarViewModel BuildTimeGrid(List<Event> events, List<DateTime> days)
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
    }
}
