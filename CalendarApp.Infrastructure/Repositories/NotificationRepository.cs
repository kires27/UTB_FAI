using CalendarApp.Domain.Entities;
using CalendarApp.Domain.Repositories;
using CalendarApp.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace CalendarApp.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly CalendarDbContext _calendarDbContext;

        public NotificationRepository(CalendarDbContext calendarDbContext)
        {
            _calendarDbContext = calendarDbContext;
        }

        public async Task<IList<Notification>> GetByUserAsync(int userId)
        {
            var notifications = await _calendarDbContext.Notifications
                .Include(n => n.Event)
                .Include(n => n.Event!.EventAttendees)
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
            return notifications;
        }

        public async Task<IList<Notification>> GetAllAsync()
        {
            var notifications = await _calendarDbContext.Notifications
                .Include(n => n.Event)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
            return notifications;
        }

        public Task<int> CountUnreadAsync(int userId)
        {
            return _calendarDbContext.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public Task<Notification?> GetByIdAsync(int id)
        {
            return _calendarDbContext.Notifications.FindAsync(id).AsTask();
        }

        public Task<Notification?> GetByIdForUserAsync(int id, int userId)
        {
            return _calendarDbContext.Notifications
                .Include(n => n.Event)
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);
        }

        public async Task AddAsync(Notification notification)
        {
            _calendarDbContext.Notifications.Add(notification);
            await _calendarDbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Notification notification)
        {
            _calendarDbContext.Notifications.Update(notification);
            await _calendarDbContext.SaveChangesAsync();
        }

        public async Task MarkInvitesReadAsync(int userId, int eventId)
        {
            var invites = await _calendarDbContext.Notifications
                .Where(n => n.UserId == userId && n.EventId == eventId && n.Type == NotificationType.EventInvite && !n.IsRead)
                .ToListAsync();
            foreach (var invite in invites)
            {
                invite.IsRead = true;
                invite.UpdatedAt = DateTime.UtcNow;
            }

            await _calendarDbContext.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var notification = await _calendarDbContext.Notifications.FindAsync(id);
            if (notification == null)
            {
                return false;
            }

            _calendarDbContext.Notifications.Remove(notification);
            await _calendarDbContext.SaveChangesAsync();
            return true;
        }
    }
}
