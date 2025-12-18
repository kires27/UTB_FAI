using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace CalendarApp.Domain.Entities
{
	public class User : IdentityUser<int>
	{	
		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime CreatedAt { get; protected set; }

		public DateTime UpdatedAt { get; set; }
	}
}