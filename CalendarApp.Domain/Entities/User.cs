using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace CalendarApp.Domain.Entities
{
	public class User : IdentityUser<int>
	{	
		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime CreatedAt { get; protected set; }

		public DateTime UpdatedAt { get; set; }

		// Navigation properties
		public virtual ICollection<Event> OwnedEvents { get; set; } = new List<Event>();
		public virtual ICollection<EventAttendee> EventAttendees { get; set; } = new List<EventAttendee>();
		public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
	}
}