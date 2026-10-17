using CalendarApp.Domain.Entities;

namespace CalendarApp.Application.Abstraction
{
    public interface IEventAppService
    {
        Task<IList<Event>> SelectAllAsync();
        Task<IList<Event>> SelectByOwnerAsync(int ownerId);
        Task<Event?> GetByIdAsync(int id);
        Task CreateAsync(Event @event);
        Task<bool> DeleteAsync(int id);
        Task UpdateAsync(Event @event);
        Task<bool> InviteUsersToEvent(int eventId, List<int> userIds);
        Task<bool> RemoveAttendeeFromEvent(int eventId, int userId);
        Task<bool> DeclineEventAsync(int eventId, int userId);
        Task<EventOccurrenceException?> GetOccurrenceExceptionAsync(int masterEventId, DateTime originalDate);
        Task UpsertOccurrenceExceptionAsync(EventOccurrenceException exception);
        Task<bool> MoveOccurrenceAsync(int eventId, DateTime? occurrenceDate, DateTime newStart, DateTime newEnd, bool allDay);
    }
}
