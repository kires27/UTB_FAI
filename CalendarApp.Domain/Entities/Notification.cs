using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CalendarApp.Domain.Entities
{
    public enum NotificationType
    {
        EventInvite,
        EventUpdate,
        Reminder,
        Custom
    }

    public class Notification : Entity<int>
    {
        [Required]
        public int UserId { get; set; }

        public int? EventId { get; set; }

        [Required]
        public NotificationType Type { get; set; }

        [Required]
        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;

        public DateTime? NotifyAt { get; set; }

    	[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime CreatedAt { get; protected set; }

        public DateTime UpdatedAt { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;

        [ForeignKey(nameof(EventId))]
        public virtual Event? Event { get; set; }
    }
}