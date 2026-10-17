using CalendarApp.Domain.Entities;

namespace CalendarApp.Domain.Repositories
{
    public interface IEventRepository
    {
        Task<IList<Event>> GetAllAsync();
        Task<IList<Event>> GetByOwnerAsync(int ownerId);
        Task<Event?> GetByIdAsync(int id);

        Task<Event> AddAsync(Event @event);
        Task<bool> UpdateAsync(Event @event);
        Task<bool> UpdateScheduleAsync(int eventId, DateTime startTime, DateTime endTime, bool allDay);
        Task<bool> DeleteAsync(int id);

        Task<EventAttendee?> GetAttendeeAsync(int eventId, int userId, bool includeEvent = false);
        Task<bool> IsAttendeeAsync(int eventId, int userId);
        Task AddAttendeeAsync(EventAttendee attendee);
        Task<bool> RemoveAttendeeAsync(int eventId, int userId);
        Task<bool> SetAttendeeStatusAsync(int eventId, int userId, AttendeeStatus status);

        Task<EventOccurrenceException?> GetOccurrenceExceptionAsync(int masterEventId, DateTime originalDate);
        Task UpsertOccurrenceExceptionAsync(EventOccurrenceException exception);
    }
}
