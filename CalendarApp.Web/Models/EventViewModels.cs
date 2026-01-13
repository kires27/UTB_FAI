using System.ComponentModel.DataAnnotations;
using CalendarApp.Domain.Entities;
using CalendarApp.Domain.Validations;
using CalendarApp.Application.Abstraction;

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

        [Required, DateRange]
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

        [StringLength(1000)]
        public string? InviteEmails { get; set; } = null;

        public List<int> RemoveAttendeeIds { get; set; } = new List<int>();
        public List<EventAttendee> CurrentAttendees { get; set; } = new List<EventAttendee>();
    }

    public class CalendarDayViewModel
    {
        public DateTime Date { get; set; }
        public bool IsCurrentMonth { get; set; }
        public bool IsToday { get; set; }
        public List<Event> Events { get; set; } = new List<Event>();
    }

    public class AdminEventCreateViewModel : EventCreateViewModel
    {
        [Required]
        [Display(Name = "Event Owner")]
        public int OwnerId { get; set; }
        
        [Display(Name = "Additional Attendees")]
        public List<int> AttendeeIds { get; set; } = new List<int>();
        
        public IList<UserWithRoleDto> AllUsers { get; set; } = new List<UserWithRoleDto>();
        public IList<UserWithRoleDto> FilteredUsers { get; set; } = new List<UserWithRoleDto>();
        public string? SearchTerm { get; set; }
    }

    public class MonthlyCalendarViewModel
    {
        public DateTime CurrentDate { get; set; }
        public DateTime DisplayMonth { get; set; }
        public List<CalendarDayViewModel> Days { get; set; } = new List<CalendarDayViewModel>();
        public string CurrentView { get; set; } = "list";
    }
}