using CalendarApp.Application.Models;
using CalendarApp.Domain.Entities;

namespace CalendarApp.Application.Abstraction
{
    // Builds the calendar view models (list, monthly grid, daily/weekly time grid).
    public interface ICalendarViewAppService
    {
        MonthlyCalendarViewModel BuildList();
        MonthlyCalendarViewModel BuildMonthlyCalendar(List<Event> events, DateTime displayDate);
        MonthlyCalendarViewModel BuildTimeGrid(List<Event> events, List<DateTime> days);
    }
}
