using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CalendarApp.Domain.Validations;

namespace CalendarApp.Domain.Entities
{
    public enum EventStatus
    {
        Confirmed,
        Tentative,
        Cancelled
    }

    public class Event : Entity<int>
    {
        [Required, TextLengthRange(3, 255)]
        public string Title { get; set; } = string.Empty;

        [TextLengthRange(0, 2000)]
        public string? Description { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required, DateRange]
        public DateTime EndTime { get; set; }

        public bool AllDay { get; set; } = false;

        [StringLength(255)]
        public string? Location { get; set; }

        public bool IsRecurring { get; set; } = false;

        [StringLength(255)]
        public string? RecurrenceRule { get; set; }

        public DateTime? RecurrenceEnd { get; set; }

        [Required]
        public int OwnerId { get; set; }

        public DateTime? ReminderTime { get; set; }

        [StringLength(7)]
        public string? Color { get; set; }

        public EventStatus Status { get; set; } = EventStatus.Confirmed;

    	[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime CreatedAt { get; protected set; }

        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey(nameof(OwnerId))]
        public virtual User Owner { get; set; } = null!;
        public virtual ICollection<EventAttendee> EventAttendees { get; set; } = new List<EventAttendee>();
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    }
}