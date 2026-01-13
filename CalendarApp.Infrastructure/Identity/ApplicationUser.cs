using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace CalendarApp.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<int>
{
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime CreatedAt { get; protected set; }

    public DateTime UpdatedAt { get; set; }
}
