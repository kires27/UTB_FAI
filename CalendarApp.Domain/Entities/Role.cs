using Microsoft.AspNetCore.Identity;

namespace CalendarApp.Domain.Entities
{
	public class Role : IdentityRole<int>
	{
		// Role inherits all properties from IdentityRole<int>
		// Including: Id, Name, NormalizedName, ConcurrencyStamp, Users, Claims
	}
}