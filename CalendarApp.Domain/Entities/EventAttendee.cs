using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CalendarApp.Domain.Entities
{
    public enum AttendeeStatus
    {
        Invited,
        Accepted,
        Declined,
        Tentative
    }

    public enum AttendeeRole
    {
        Owner,
        Participant
    }

    public class EventAttendee
    {
        [Required]
        public int EventId { get; set; }

        [Required]
        public int UserId { get; set; }

        public AttendeeStatus Status { get; set; } = AttendeeStatus.Invited;

        public AttendeeRole Role { get; set; } = AttendeeRole.Participant;

    	[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime CreatedAt { get; protected set; }

        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey(nameof(EventId))]
        public virtual Event Event { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;
    }
}