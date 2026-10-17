using CalendarApp.Application.Models;
using CalendarApp.Domain.Entities;

namespace CalendarApp.Application.Abstraction
{
    public interface INotificationAppService
    {
        Task<IList<Notification>> GetUserNotificationsAsync(int userId);
        Task<IList<AdminNotificationViewModel>> SelectAllForAdminAsync();
        Task<bool> DeleteAsync(int id);
        Task<int> GetUnreadCountAsync(int userId);
        Task<bool> MarkAsReadAsync(int notificationId, int userId);
        Task<bool> AcceptInvitationAsync(int notificationId, int userId);
        Task<bool> DeclineInvitationAsync(int notificationId, int userId);
    }
}