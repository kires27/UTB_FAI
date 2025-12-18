using System.ComponentModel.DataAnnotations;
using CalendarApp.Domain.Entities;

namespace CalendarApp.Web.Models
{
    public class EventCreateViewModel
    {
        [Required, StringLength(255)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        [Required]
        public DateTime StartTime { get; set; } = DateTime.Now;

        [Required]
        public DateTime EndTime { get; set; } = DateTime.Now.AddHours(1);

        public bool AllDay { get; set; } = false;

        [StringLength(255)]
        public string? Location { get; set; }

        public bool IsRecurring { get; set; } = false;

        [StringLength(255)]
        public string? RecurrenceRule { get; set; }

        public DateTime? RecurrenceEnd { get; set; }

        public DateTime? ReminderTime { get; set; }

        [StringLength(7)]
        public string? Color { get; set; } = "#3788d8";

        public EventStatus Status { get; set; } = EventStatus.Confirmed;

        [StringLength(1000)]
        public string? InviteEmails { get; set; } = null;
    }

    public class EventEditViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required, StringLength(255)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
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

        [StringLength(1000)]
        public string? InviteEmails { get; set; } = null;

        public List<int> RemoveAttendeeIds { get; set; } = new List<int>();
        public List<EventAttendee> CurrentAttendees { get; set; } = new List<EventAttendee>();
    }
}