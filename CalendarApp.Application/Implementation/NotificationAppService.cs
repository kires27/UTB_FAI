using CalendarApp.Application.Abstraction;
using CalendarApp.Domain.Entities;
using CalendarApp.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace CalendarApp.Application.Implementation
{
    public class NotificationAppService : INotificationAppService
    {
        private readonly CalendarDbContext _calendarDbContext;

        public NotificationAppService(CalendarDbContext calendarDbContext)
        {
            _calendarDbContext = calendarDbContext;
        }

        public async Task<IList<Notification>> GetUserNotificationsAsync(int userId)
        {
            return await _calendarDbContext.Notifications
                .Include(n => n.Event)
                .Include(n => n.Event!.Owner)
                .Include(n => n.Event!.EventAttendees)
                    .ThenInclude(ea => ea.User)
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _calendarDbContext.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task<bool> MarkAsReadAsync(int notificationId)
        {
            var notification = await _calendarDbContext.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                notification.UpdatedAt = DateTime.UtcNow;
                await _calendarDbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> AcceptInvitationAsync(int notificationId, int userId)
        {
            var notification = await _calendarDbContext.Notifications
                .Include(n => n.Event)
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification == null || notification.Event == null)
                return false;

            // Update EventAttendee status to Accepted
            var eventAttendee = await _calendarDbContext.EventAttendees
                .FirstOrDefaultAsync(ea => ea.EventId == notification.EventId && ea.UserId == userId);

            if (eventAttendee != null)
            {
                eventAttendee.Status = AttendeeStatus.Accepted;
                eventAttendee.UpdatedAt = DateTime.UtcNow;
            }

            // Mark notification as read
            notification.IsRead = true;
            notification.UpdatedAt = DateTime.UtcNow;

            await _calendarDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeclineInvitationAsync(int notificationId, int userId)
        {
            var notification = await _calendarDbContext.Notifications
                .Include(n => n.Event)
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification == null || notification.Event == null)
                return false;

            // Update EventAttendee status to Declined
            var eventAttendee = await _calendarDbContext.EventAttendees
                .FirstOrDefaultAsync(ea => ea.EventId == notification.EventId && ea.UserId == userId);

            if (eventAttendee != null)
            {
                eventAttendee.Status = AttendeeStatus.Declined;
                eventAttendee.UpdatedAt = DateTime.UtcNow;
            }

            // Mark notification as read
            notification.IsRead = true;
            notification.UpdatedAt = DateTime.UtcNow;

            await _calendarDbContext.SaveChangesAsync();
            return true;
        }
    }
}