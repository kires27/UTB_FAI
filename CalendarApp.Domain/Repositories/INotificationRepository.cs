using CalendarApp.Domain.Entities;

namespace CalendarApp.Domain.Repositories
{
    // Notification persistence contract; implemented in Infrastructure, consumed by application services.
    public interface INotificationRepository
    {
        Task<IList<Notification>> GetByUserAsync(int userId);
        Task<IList<Notification>> GetAllAsync();
        Task<int> CountUnreadAsync(int userId);
        Task<Notification?> GetByIdAsync(int id);
        Task<Notification?> GetByIdForUserAsync(int id, int userId);
        Task AddAsync(Notification notification);
        Task UpdateAsync(Notification notification);
        Task MarkInvitesReadAsync(int userId, int eventId);
        Task<bool> DeleteAsync(int id);
    }
}
