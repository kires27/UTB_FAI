using CalendarApp.Domain.Entities;
using CalendarApp.Infrastructure.Database;

namespace CalendarApp.Infrastructure.Database.Seeding
{
    public static class DataSeeder
    {
        public static async Task SeededDataAsync(CalendarDbContext dbContext)
        {
            // Check if data already exists
            if (dbContext.Users.Any())
            {
                return; // Data already seeded
            }

            // Create users with plain text passwords (will be hashed)
            var users = new List<User>
            {
                new User
                {
                    UserName = "alexjohnson",
                    Email = "alex.johnson@email.com",
                    FullName = "Alex Johnson",
                    PasswordHash = HashPassword("User123!"),
                    Role = "User",
                    CreatedAt = new DateTime(2025, 11, 7, 0, 0, 0),
                    UpdatedAt = new DateTime(2025, 11, 7, 0, 0, 0)
                },
                new User
                {
                    UserName = "sarahchen",
                    Email = "sarah.chen@email.com",
                    FullName = "Sarah Chen",
                    PasswordHash = HashPassword("User123!"),
                    Role = "User",
                    CreatedAt = new DateTime(2025, 11, 12, 0, 0, 0),
                    UpdatedAt = new DateTime(2025, 11, 12, 0, 0, 0)
                },
                new User
                {
                    UserName = "mikedavis",
                    Email = "mike.davis@email.com",
                    FullName = "Mike Davis",
                    PasswordHash = HashPassword("User123!"),
                    Role = "User",
                    CreatedAt = new DateTime(2025, 11, 17, 0, 0, 0),
                    UpdatedAt = new DateTime(2025, 11, 17, 0, 0, 0)
                },
                new User
                {
                    UserName = "admin",
                    Email = "admin@calendarapp.com",
                    FullName = "System Administrator",
                    PasswordHash = HashPassword("Admin123!"),
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            await dbContext.Users.AddRangeAsync(users);

            // Create events
            var events = new List<Event>
            {
                new Event
                {
                    Title = "Q4 Planning Session",
                    Description = "Quarterly business planning and strategy meeting",
                    StartTime = new DateTime(2025, 12, 8, 9, 0, 0),
                    EndTime = new DateTime(2025, 12, 8, 11, 0, 0),
                    Location = "Conference Room A",
                    AllDay = false,
                    IsRecurring = false,
                    OwnerId = 1, // Alex
                    Visibility = EventVisibility.Public,
                    Status = EventStatus.Confirmed,
                    Color = "#007bff",
                    CreatedAt = new DateTime(2025, 11, 30, 0, 0, 0),
                    UpdatedAt = new DateTime(2025, 11, 30, 0, 0, 0)
                },
                new Event
                {
                    Title = "Grocery Shopping",
                    Description = "Weekly grocery run",
                    StartTime = new DateTime(2025, 12, 13, 14, 0, 0),
                    EndTime = new DateTime(2025, 12, 13, 15, 0, 0),
                    Location = "Supermarket",
                    AllDay = false,
                    IsRecurring = true,
                    RecurrenceRule = "FREQ=WEEKLY;BYDAY=SA",
                    OwnerId = 2, // Sarah
                    Visibility = EventVisibility.Private,
                    Status = EventStatus.Confirmed,
                    Color = "#28a745",
                    CreatedAt = new DateTime(2025, 11, 23, 0, 0, 0),
                    UpdatedAt = new DateTime(2025, 11, 23, 0, 0, 0)
                },
                new Event
                {
                    Title = "Friday Night Drinks",
                    Description = "Team happy hour after work",
                    StartTime = new DateTime(2025, 12, 12, 19, 0, 0),
                    EndTime = new DateTime(2025, 12, 12, 22, 0, 0),
                    Location = "The Local Pub",
                    AllDay = false,
                    IsRecurring = false,
                    OwnerId = 1, // Alex
                    Visibility = EventVisibility.Shared,
                    Status = EventStatus.Confirmed,
                    Color = "#ffc107",
                    CreatedAt = new DateTime(2025, 12, 4, 0, 0, 0),
                    UpdatedAt = new DateTime(2025, 12, 4, 0, 0, 0)
                },
                new Event
                {
                    Title = "Morning Workout",
                    Description = "Cardio and strength training",
                    StartTime = new DateTime(2025, 12, 9, 6, 30, 0),
                    EndTime = new DateTime(2025, 12, 9, 7, 30, 0),
                    Location = "Gym",
                    AllDay = false,
                    IsRecurring = true,
                    RecurrenceRule = "FREQ=WEEKLY;BYDAY=TU,TH",
                    OwnerId = 3, // Mike
                    Visibility = EventVisibility.Private,
                    Status = EventStatus.Confirmed,
                    Color = "#dc3545",
                    CreatedAt = new DateTime(2025, 11, 7, 0, 0, 0),
                    UpdatedAt = new DateTime(2025, 11, 7, 0, 0, 0)
                }
            };

            await dbContext.Events.AddRangeAsync(events);

            // Create event attendees
            var eventAttendees = new List<EventAttendee>
            {
                new EventAttendee
                {
                    EventId = 3, // Friday Night Drinks
                    UserId = 2, // Sarah
                    Status = AttendeeStatus.Accepted,
                    Role = AttendeeRole.Participant,
                    CreatedAt = new DateTime(2025, 12, 4, 0, 0, 0),
                    UpdatedAt = new DateTime(2025, 12, 5, 0, 0, 0)
                },
                new EventAttendee
                {
                    EventId = 3, // Friday Night Drinks
                    UserId = 3, // Mike
                    Status = AttendeeStatus.Tentative,
                    Role = AttendeeRole.Participant,
                    CreatedAt = new DateTime(2025, 12, 4, 0, 0, 0),
                    UpdatedAt = new DateTime(2025, 12, 6, 0, 0, 0)
                },
                new EventAttendee
                {
                    EventId = 6, // Birthday Party
                    UserId = 1, // Alex
                    Status = AttendeeStatus.Accepted,
                    Role = AttendeeRole.Participant,
                    CreatedAt = new DateTime(2025, 12, 2, 0, 0, 0),
                    UpdatedAt = new DateTime(2025, 12, 3, 0, 0, 0)
                }
            };

            await dbContext.EventAttendees.AddRangeAsync(eventAttendees);

            await dbContext.SaveChangesAsync();
        }

        private static string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}