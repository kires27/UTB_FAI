using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using CalendarApp.Domain.Entities;
using CalendarApp.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CalendarApp.Infrastructure.Database
{
	public class CalendarDbContext : IdentityDbContext<User, Role, int>
	{
		public DbSet<Event> Events { get; set; }
		public DbSet<EventAttendee> EventAttendees { get; set; }
		public DbSet<Notification> Notifications { get; set; }
		public DbSet<EventOccurrenceException> EventOccurrenceExceptions { get; set; }

		public CalendarDbContext(DbContextOptions options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Event>()
				.HasOne<User>()
				.WithMany()
				.HasForeignKey(e => e.OwnerId);

			modelBuilder.Entity<EventAttendee>()
        		.HasKey(ea => new { ea.EventId, ea.UserId });

			modelBuilder.Entity<EventAttendee>()
				.HasOne<User>()
				.WithMany()
				.HasForeignKey(ea => ea.UserId);

			modelBuilder.Entity<Notification>()
				.HasOne<User>()
				.WithMany()
				.HasForeignKey(n => n.UserId);

			modelBuilder.Entity<EventOccurrenceException>()
				.HasOne(x => x.MasterEvent)
				.WithMany(e => e.Exceptions)
				.HasForeignKey(x => x.MasterEventId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<EventOccurrenceException>()
				.HasIndex(x => new { x.MasterEventId, x.OriginalDate })
				.IsUnique();

			SeedData(modelBuilder);
		}

		private static void SeedData(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Role>().HasData(
				new { Id = 1, Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = "439ed2ac-e661-4015-9964-e15b29afcdac" },
				new { Id = 2, Name = "User", NormalizedName = "USER", ConcurrencyStamp = "fd9ceb09-569e-47d5-b17a-c30ad8117f24" });

			modelBuilder.Entity<User>().HasData(
				new
				{
					Id = -1,
					UserName = "admin",
					NormalizedUserName = "ADMIN",
					Email = "admin@calendarapp.com",
					NormalizedEmail = "ADMIN@CALENDARAPP.COM",
					EmailConfirmed = true,
					PasswordHash = "AQAAAAEAACcQAAAAEKOgH5peMUVS21nMyfE9tbpcPn6U31VL+vpcE3Y/wMAqvGY4JPaPsBV6MFZBx2KPQw==",
					SecurityStamp = "c670b2c6-19b9-4603-9de7-6490e7b1b734",
					ConcurrencyStamp = "ceec1c53-5dcc-4c96-bd72-51cfd20c3fe6",
					PhoneNumberConfirmed = false,
					TwoFactorEnabled = false,
					LockoutEnabled = true,
					AccessFailedCount = 0,
					CreatedAt = new DateTime(2026, 9, 7),
					UpdatedAt = new DateTime(2026, 9, 7)
				},
				new
				{
					Id = -2,
					UserName = "alexjohnson",
					NormalizedUserName = "ALEXJOHNSON",
					Email = "alex.johnson@email.com",
					NormalizedEmail = "ALEX.JOHNSON@EMAIL.COM",
					EmailConfirmed = true,
					PasswordHash = "AQAAAAEAACcQAAAAECGadj9TUQS2u76DLVVo/kkCqwpE1KhvYmrPbxvqQlST8uXxecrCduBUsoGbnC7vTQ==",
					SecurityStamp = "d71c3ace-94d7-4576-9d7b-9c5aa98c5a7c",
					ConcurrencyStamp = "309cf6b3-a538-47f8-acd7-740ee59362b6",
					PhoneNumberConfirmed = false,
					TwoFactorEnabled = false,
					LockoutEnabled = true,
					AccessFailedCount = 0,
					CreatedAt = new DateTime(2026, 9, 7),
					UpdatedAt = new DateTime(2026, 9, 7)
				},
				new
				{
					Id = -3,
					UserName = "sarahchen",
					NormalizedUserName = "SARAHCHEN",
					Email = "sarah.chen@email.com",
					NormalizedEmail = "SARAH.CHEN@EMAIL.COM",
					EmailConfirmed = true,
					PasswordHash = "AQAAAAEAACcQAAAAECGadj9TUQS2u76DLVVo/kkCqwpE1KhvYmrPbxvqQlST8uXxecrCduBUsoGbnC7vTQ==",
					SecurityStamp = "5904b64c-0089-4b6f-a5cd-cd3fb2a4947b",
					ConcurrencyStamp = "48037745-2d59-43a2-b196-d384b9187764",
					PhoneNumberConfirmed = false,
					TwoFactorEnabled = false,
					LockoutEnabled = true,
					AccessFailedCount = 0,
					CreatedAt = new DateTime(2026, 9, 12),
					UpdatedAt = new DateTime(2026, 9, 12)
				},
				new
				{
					Id = -4,
					UserName = "mikedavis",
					NormalizedUserName = "MIKEDAVIS",
					Email = "mike.davis@email.com",
					NormalizedEmail = "MIKE.DAVIS@EMAIL.COM",
					EmailConfirmed = true,
					PasswordHash = "AQAAAAEAACcQAAAAECGadj9TUQS2u76DLVVo/kkCqwpE1KhvYmrPbxvqQlST8uXxecrCduBUsoGbnC7vTQ==",
					SecurityStamp = "0253c2be-83bf-4bde-a547-0ea099be7c10",
					ConcurrencyStamp = "8bb62e2b-1506-42fe-b72e-3dac0cd41c64",
					PhoneNumberConfirmed = false,
					TwoFactorEnabled = false,
					LockoutEnabled = true,
					AccessFailedCount = 0,
					CreatedAt = new DateTime(2026, 9, 17),
					UpdatedAt = new DateTime(2026, 9, 17)
				});

			modelBuilder.Entity<IdentityUserRole<int>>().HasData(
				new { UserId = -1, RoleId = 1 },
				new { UserId = -2, RoleId = 2 },
				new { UserId = -3, RoleId = 2 },
				new { UserId = -4, RoleId = 2 });

			modelBuilder.Entity<Event>().HasData(
				new
				{
					Id = -1,
					Title = "Q4 Planning Session",
					Description = "Quarterly business planning and strategy meeting",
					StartTime = new DateTime(2026, 12, 15, 9, 0, 0),
					EndTime = new DateTime(2026, 12, 15, 11, 0, 0),
					Location = "Conference Room A",
					AllDay = false,
					IsRecurring = false,
					OwnerId = -1,
					Status = EventStatus.Confirmed,
					Color = "#007bff",
					CreatedAt = new DateTime(2026, 11, 30),
					UpdatedAt = new DateTime(2026, 11, 30)
				},
				new
				{
					Id = -2,
					Title = "Client Call - Project X",
					Description = "Discuss project requirements and timeline",
					StartTime = new DateTime(2026, 12, 18, 11, 0, 0),
					EndTime = new DateTime(2026, 12, 18, 12, 0, 0),
					Location = "Virtual - Zoom",
					AllDay = false,
					IsRecurring = false,
					OwnerId = -1,
					Status = EventStatus.Confirmed,
					Color = "#007bff",
					CreatedAt = new DateTime(2026, 12, 5),
					UpdatedAt = new DateTime(2026, 12, 5)
				},
				new
				{
					Id = -3,
					Title = "Grocery Shopping",
					Description = "Weekly grocery run",
					StartTime = new DateTime(2026, 12, 20, 14, 0, 0),
					EndTime = new DateTime(2026, 12, 20, 15, 0, 0),
					Location = "Supermarket",
					AllDay = false,
					IsRecurring = true,
					RecurrenceRule = "FREQ=WEEKLY;BYDAY=SA",
					OwnerId = -2,
					Status = EventStatus.Confirmed,
					Color = "#28a745",
					CreatedAt = new DateTime(2026, 11, 23),
					UpdatedAt = new DateTime(2026, 11, 23)
				},
				new
				{
					Id = -4,
					Title = "Dentist Appointment",
					Description = "Regular dental checkup",
					StartTime = new DateTime(2026, 12, 17, 15, 0, 0),
					EndTime = new DateTime(2026, 12, 17, 16, 0, 0),
					Location = "Dental Clinic",
					AllDay = false,
					IsRecurring = false,
					OwnerId = -2,
					Status = EventStatus.Confirmed,
					Color = "#6f42c1",
					CreatedAt = new DateTime(2026, 11, 27),
					UpdatedAt = new DateTime(2026, 11, 27)
				},
				new
				{
					Id = -5,
					Title = "Morning Workout",
					Description = "Cardio and strength training",
					StartTime = new DateTime(2026, 12, 16, 6, 30, 0),
					EndTime = new DateTime(2026, 12, 16, 7, 30, 0),
					Location = "Gym",
					AllDay = false,
					IsRecurring = true,
					RecurrenceRule = "FREQ=WEEKLY;BYDAY=TU,TH",
					OwnerId = -3,
					Status = EventStatus.Confirmed,
					Color = "#dc3545",
					CreatedAt = new DateTime(2026, 11, 7),
					UpdatedAt = new DateTime(2026, 11, 7)
				},
				new
				{
					Id = -6,
					Title = "Friday Night Drinks",
					Description = "Team happy hour after work",
					StartTime = new DateTime(2026, 12, 19, 19, 0, 0),
					EndTime = new DateTime(2026, 12, 19, 22, 0, 0),
					Location = "The Local Pub",
					AllDay = false,
					IsRecurring = false,
					OwnerId = -1,
					Status = EventStatus.Confirmed,
					Color = "#ffc107",
					CreatedAt = new DateTime(2026, 12, 4),
					UpdatedAt = new DateTime(2026, 12, 4)
				},
				new
				{
					Id = -7,
					Title = "Birthday Party - Emma",
					Description = "Emma's 30th birthday celebration",
					StartTime = new DateTime(2026, 12, 20, 18, 0, 0),
					EndTime = new DateTime(2026, 12, 20, 23, 0, 0),
					Location = "Emma's House",
					AllDay = false,
					IsRecurring = false,
					OwnerId = -2,
					Status = EventStatus.Confirmed,
					Color = "#e83e8c",
					CreatedAt = new DateTime(2026, 12, 2),
					UpdatedAt = new DateTime(2026, 12, 2)
				},
				new
				{
					Id = -8,
					Title = "Lunch with Mom",
					Description = "Monthly catch-up lunch",
					StartTime = new DateTime(2026, 12, 21, 12, 30, 0),
					EndTime = new DateTime(2026, 12, 21, 14, 0, 0),
					Location = "Favorite Restaurant",
					AllDay = false,
					IsRecurring = true,
					RecurrenceRule = "FREQ=MONTHLY;BYMONTHDAY=15",
					OwnerId = -3,
					Status = EventStatus.Confirmed,
					Color = "#fd7e14",
					CreatedAt = new DateTime(2026, 11, 22),
					UpdatedAt = new DateTime(2026, 11, 22)
				},
				new
				{
					Id = -9,
					Title = "Team Standup",
					Description = "Daily team sync",
					StartTime = new DateTime(2026, 11, 2, 9, 0, 0),
					EndTime = new DateTime(2026, 11, 2, 9, 30, 0),
					Location = "Office - Room B",
					AllDay = false,
					IsRecurring = true,
					RecurrenceRule = "FREQ=WEEKLY;BYDAY=MO,WE,FR",
					OwnerId = -4,
					Status = EventStatus.Confirmed,
					Color = "#17a2b8",
					CreatedAt = new DateTime(2026, 10, 1),
					UpdatedAt = new DateTime(2026, 10, 1)
				},
				new
				{
					Id = -10,
					Title = "Car Service",
					Description = "Annual car maintenance",
					StartTime = new DateTime(2026, 11, 14, 8, 0, 0),
					EndTime = new DateTime(2026, 11, 14, 10, 0, 0),
					Location = "Auto Service Center",
					AllDay = false,
					IsRecurring = false,
					OwnerId = -4,
					Status = EventStatus.Tentative,
					Color = "#fd7e14",
					CreatedAt = new DateTime(2026, 10, 5),
					UpdatedAt = new DateTime(2026, 10, 5)
				},
				new
				{
					Id = -11,
					Title = "Weekend Hiking Trip",
					Description = "Day hike with friends, bring water and snacks",
					StartTime = new DateTime(2026, 11, 21, 8, 0, 0),
					EndTime = new DateTime(2026, 11, 21, 17, 0, 0),
					Location = "Mountain Trail",
					AllDay = false,
					IsRecurring = false,
					OwnerId = -4,
					Status = EventStatus.Confirmed,
					Color = "#20c997",
					CreatedAt = new DateTime(2026, 10, 8),
					UpdatedAt = new DateTime(2026, 10, 8)
				});

			modelBuilder.Entity<EventAttendee>().HasData(
				new
				{
					EventId = -6,
					UserId = -2,
					Status = AttendeeStatus.Accepted,
					Role = AttendeeRole.Participant,
					CreatedAt = new DateTime(2026, 12, 5),
					UpdatedAt = new DateTime(2026, 12, 5)
				},
				new
				{
					EventId = -6,
					UserId = -3,
					Status = AttendeeStatus.Tentative,
					Role = AttendeeRole.Participant,
					CreatedAt = new DateTime(2026, 12, 6),
					UpdatedAt = new DateTime(2026, 12, 6)
				},
				new
				{
					EventId = -7,
					UserId = -1,
					Status = AttendeeStatus.Accepted,
					Role = AttendeeRole.Participant,
					CreatedAt = new DateTime(2026, 12, 3),
					UpdatedAt = new DateTime(2026, 12, 3)
				},
				new
				{
					EventId = -5,
					UserId = -2,
					Status = AttendeeStatus.Invited,
					Role = AttendeeRole.Participant,
					CreatedAt = new DateTime(2026, 11, 30),
					UpdatedAt = new DateTime(2026, 11, 30)
				},
				new
				{
					EventId = -1,
					UserId = -1,
					Status = AttendeeStatus.Accepted,
					Role = AttendeeRole.Owner,
					CreatedAt = new DateTime(2026, 11, 30),
					UpdatedAt = new DateTime(2026, 11, 30)
				},
				new
				{
					EventId = -3,
					UserId = -2,
					Status = AttendeeStatus.Accepted,
					Role = AttendeeRole.Owner,
					CreatedAt = new DateTime(2026, 11, 23),
					UpdatedAt = new DateTime(2026, 11, 23)
				},
				new
				{
					EventId = -9,
					UserId = -4,
					Status = AttendeeStatus.Accepted,
					Role = AttendeeRole.Owner,
					CreatedAt = new DateTime(2026, 10, 1),
					UpdatedAt = new DateTime(2026, 10, 1)
				},
				new
				{
					EventId = -10,
					UserId = -4,
					Status = AttendeeStatus.Accepted,
					Role = AttendeeRole.Owner,
					CreatedAt = new DateTime(2026, 10, 5),
					UpdatedAt = new DateTime(2026, 10, 5)
				},
				new
				{
					EventId = -11,
					UserId = -4,
					Status = AttendeeStatus.Accepted,
					Role = AttendeeRole.Owner,
					CreatedAt = new DateTime(2026, 10, 8),
					UpdatedAt = new DateTime(2026, 10, 8)
				},
				new
				{
					EventId = -11,
					UserId = -2,
					Status = AttendeeStatus.Accepted,
					Role = AttendeeRole.Participant,
					CreatedAt = new DateTime(2026, 10, 9),
					UpdatedAt = new DateTime(2026, 10, 9)
				},
				new
				{
					EventId = -11,
					UserId = -3,
					Status = AttendeeStatus.Invited,
					Role = AttendeeRole.Participant,
					CreatedAt = new DateTime(2026, 10, 8),
					UpdatedAt = new DateTime(2026, 10, 8)
				});

			modelBuilder.Entity<Notification>().HasData(
				new
				{
					Id = -1,
					UserId = -2,
					EventId = (int?)-6,
					Type = NotificationType.EventInvite,
					Message = "Alex Johnson invited you to 'Friday Night Drinks'",
					IsRead = true,
					NotifyAt = (DateTime?)new DateTime(2026, 12, 4),
					CreatedAt = new DateTime(2026, 12, 5),
					UpdatedAt = new DateTime(2026, 12, 5)
				},
				new
				{
					Id = -2,
					UserId = -3,
					EventId = (int?)-6,
					Type = NotificationType.EventInvite,
					Message = "Alex Johnson invited you to 'Friday Night Drinks'",
					IsRead = false,
					NotifyAt = (DateTime?)new DateTime(2026, 12, 4),
					CreatedAt = new DateTime(2026, 12, 4),
					UpdatedAt = new DateTime(2026, 12, 4)
				},
				new
				{
					Id = -3,
					UserId = -1,
					EventId = (int?)-7,
					Type = NotificationType.EventInvite,
					Message = "Sarah Chen invited you to 'Birthday Party - Emma'",
					IsRead = true,
					NotifyAt = (DateTime?)new DateTime(2026, 12, 2),
					CreatedAt = new DateTime(2026, 12, 3),
					UpdatedAt = new DateTime(2026, 12, 3)
				},
				new
				{
					Id = -5,
					UserId = -1,
					EventId = (int?)-2,
					Type = NotificationType.EventUpdate,
					Message = "Event 'Client Call - Project X' has been confirmed",
					IsRead = true,
					NotifyAt = (DateTime?)new DateTime(2026, 12, 5),
					CreatedAt = new DateTime(2026, 12, 5),
					UpdatedAt = new DateTime(2026, 12, 5)
				},
				new
				{
					Id = -6,
					UserId = -3,
					EventId = (int?)null,
					Type = NotificationType.Custom,
					Message = "Don't forget to bring your gym card for tomorrow's workout!",
					IsRead = false,
					NotifyAt = (DateTime?)new DateTime(2026, 12, 15, 10, 0, 0),
					CreatedAt = new DateTime(2026, 12, 15, 10, 0, 0),
					UpdatedAt = new DateTime(2026, 12, 15, 10, 0, 0)
				},
				new
				{
					Id = -7,
					UserId = -2,
					EventId = (int?)-11,
					Type = NotificationType.EventInvite,
					Message = "Mike Davis invited you to 'Weekend Hiking Trip'",
					IsRead = false,
					NotifyAt = (DateTime?)new DateTime(2026, 10, 8),
					CreatedAt = new DateTime(2026, 10, 8),
					UpdatedAt = new DateTime(2026, 10, 8)
				});
		}
	}
}
