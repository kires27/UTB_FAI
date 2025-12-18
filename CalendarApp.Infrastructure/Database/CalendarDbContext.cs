using Microsoft.EntityFrameworkCore;
using CalendarApp.Domain.Entities;
using CalendarApp.Infrastructure.Database.Seeding;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CalendarApp.Infrastructure.Database
{
	public class CalendarDbContext : IdentityDbContext<User, Role, int>
	{
		public DbSet<Event> Events { get; set; }
		public DbSet<EventAttendee> EventAttendees { get; set; }
		public DbSet<Notification> Notifications { get; set; }

		public CalendarDbContext(DbContextOptions options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<EventAttendee>()
        		.HasKey(ea => new { ea.EventId, ea.UserId });

		}
	}
}