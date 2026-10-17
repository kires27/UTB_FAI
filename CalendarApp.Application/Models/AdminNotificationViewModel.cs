using CalendarApp.Domain.Entities;

namespace CalendarApp.Application.Models
{
    public class AdminNotificationViewModel
    {
        public Notification Notification { get; set; } = null!;

        public string RecipientUserName { get; set; } = string.Empty;
    }
}
