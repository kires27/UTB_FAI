using CalendarApp.Domain.Entities;

namespace CalendarApp.Application.Abstraction
{
    public interface INotificationAppService
    {
        Task<IList<Notification>> GetUserNotificationsAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);
        Task<bool> MarkAsReadAsync(int notificationId);
        Task<bool> AcceptInvitationAsync(int notificationId, int userId);
        Task<bool> DeclineInvitationAsync(int notificationId, int userId);
    }
}