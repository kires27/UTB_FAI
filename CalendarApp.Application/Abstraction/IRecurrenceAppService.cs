using CalendarApp.Domain.Entities;

namespace CalendarApp.Application.Abstraction
{
    public interface IRecurrenceAppService
    {
        List<Event> Expand(IEnumerable<Event> events, DateTime rangeStart, DateTime rangeEnd);
        IEnumerable<Event> Expand(Event @event, DateTime rangeStart, DateTime rangeEnd);
        string? Describe(Event @event);
    }
}
