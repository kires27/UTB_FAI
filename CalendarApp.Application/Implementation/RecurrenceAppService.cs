using CalendarApp.Application.Abstraction;
using CalendarApp.Application.Helpers;
using CalendarApp.Domain.Entities;

namespace CalendarApp.Application.Implementation
{
    public class RecurrenceAppService : IRecurrenceAppService
    {
        public List<Event> Expand(IEnumerable<Event> events, DateTime rangeStart, DateTime rangeEnd)
        {
            return RecurrenceExpander.Expand(events, rangeStart, rangeEnd);
        }

        public IEnumerable<Event> Expand(Event @event, DateTime rangeStart, DateTime rangeEnd)
        {
            return RecurrenceExpander.Expand(@event, rangeStart, rangeEnd);
        }

        public string? Describe(Event @event)
        {
            return RecurrenceExpander.Describe(@event);
        }
    }
}
