using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CalendarApp.Domain.Entities
{
    // Stores a per-occurrence override of a recurring event: an instance that was
    // edited or deleted ("this occurrence only"). A row exists only for occurrences
    // that differ from the series; null fields inherit the master event's values.
    public class EventOccurrenceException : Entity<int>
    {
        [Required]
        public int MasterEventId { get; set; }

        // Date of the affected occurrence within the series (time-of-day 00:00)
        [Required]
        public DateTime OriginalDate { get; set; }

        // True when the user deleted only this occurrence
        public bool IsCancelled { get; set; }

        // Field overrides — null = inherit from the master event
        public string? Title { get; set; }

        [StringLength(2000)]
        public string? Description { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public bool? AllDay { get; set; }

        [StringLength(255)]
        public string? Location { get; set; }

        public DateTime? ReminderTime { get; set; }

        [StringLength(7)]
        public string? Color { get; set; }

        public EventStatus? Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey(nameof(MasterEventId))]
        public virtual Event MasterEvent { get; set; } = null!;
    }
}
