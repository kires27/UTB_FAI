using CalendarApp.Domain.Entities;
using CalendarApp.Infrastructure.Identity;

namespace CalendarApp.Application.Abstraction
{
    public interface IEventAppService
    {
        IList<Event> SelectAll();
        Event? GetById(int id);
        void Create(Event @event);
        bool Delete(int id);
        void Update(Event @event);
        Task<bool> InviteUsersToEvent(int eventId, List<int> userIds);
        Dictionary<int, User> GetUsersByIds(IEnumerable<int> userIds);
        Task<List<User>> FindUsersByEmailsAsync(List<string> emails);
        Task<List<User>> SearchUsersAsync(string term);
        Task<bool> RemoveAttendeeFromEvent(int eventId, int userId);
        Task<bool> DeclineEventAsync(int eventId, int userId);
        EventOccurrenceException? GetOccurrenceException(int masterEventId, DateTime originalDate);
        void UpsertOccurrenceException(EventOccurrenceException exception);
    }
}