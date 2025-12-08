using CalendarApp.Domain.Entities;

namespace CalendarApp.Infrastructure.Database.Seeding
{
    internal class CalendarDataInit
    {
        // Users are now created by RoleSeeder with proper Identity password hashing
        // This method is removed to avoid conflicts and security issues

        public List<Event> GenerateEvents()
        {
            var baseDate = new DateTime(2025, 12, 7, 0, 0, 0); // Static date for seeding

            return new List<Event>
            {
                // Business Meeting - Monday 9AM
                new Event
                {
                    Id = 1,
                    Title = "Q4 Planning Session",
                    Description = "Quarterly business planning and strategy meeting",
                    StartTime = baseDate.AddDays(-(int)baseDate.DayOfWeek + 1).AddHours(9),
                    EndTime = baseDate.AddDays(-(int)baseDate.DayOfWeek + 1).AddHours(11),
                    Location = "Conference Room A",
                    AllDay = false,
                    IsRecurring = false,
                    OwnerId = 1,
                    Visibility = EventVisibility.Public,
                    Status = EventStatus.Confirmed,
                    Color = "#007bff",
                    CreatedAt = new DateTime(2025, 11, 30),
                    UpdatedAt = new DateTime(2025, 11, 30)
                },
                // Shopping - Saturday 2PM
                new Event
                {
                    Id = 2,
                    Title = "Grocery Shopping",
                    Description = "Weekly grocery run",
                    StartTime = baseDate.AddDays(6 - (int)baseDate.DayOfWeek).AddHours(14),
                    EndTime = baseDate.AddDays(6 - (int)baseDate.DayOfWeek).AddHours(15),
                    Location = "Supermarket",
                    AllDay = false,
                    IsRecurring = true,
                    RecurrenceRule = "FREQ=WEEKLY;BYDAY=SA",
                    OwnerId = 2,
                    Visibility = EventVisibility.Private,
                    Status = EventStatus.Confirmed,
                    Color = "#28a745",
                    CreatedAt = new DateTime(2025, 11, 23),
                    UpdatedAt = new DateTime(2025, 11, 23)
                },
                // Social - Friday 7PM
                new Event
                {
                    Id = 3,
                    Title = "Friday Night Drinks",
                    Description = "Team happy hour after work",
                    StartTime = baseDate.AddDays(5 - (int)baseDate.DayOfWeek).AddHours(19),
                    EndTime = baseDate.AddDays(5 - (int)baseDate.DayOfWeek).AddHours(22),
                    Location = "The Local Pub",
                    AllDay = false,
                    IsRecurring = false,
                    OwnerId = 1,
                    Visibility = EventVisibility.Shared,
                    Status = EventStatus.Confirmed,
                    Color = "#ffc107",
                    CreatedAt = new DateTime(2025, 12, 4),
                    UpdatedAt = new DateTime(2025, 12, 4)
                },
                // Fitness - Tuesday 6:30AM
                new Event
                {
                    Id = 4,
                    Title = "Morning Workout",
                    Description = "Cardio and strength training",
                    StartTime = baseDate.AddDays(2 - (int)baseDate.DayOfWeek).AddHours(6).AddMinutes(30),
                    EndTime = baseDate.AddDays(2 - (int)baseDate.DayOfWeek).AddHours(7).AddMinutes(30),
                    Location = "Gym",
                    AllDay = false,
                    IsRecurring = true,
                    RecurrenceRule = "FREQ=WEEKLY;BYDAY=TU,TH",
                    OwnerId = 3,
                    Visibility = EventVisibility.Private,
                    Status = EventStatus.Confirmed,
                    Color = "#dc3545",
                    CreatedAt = new DateTime(2025, 11, 7),
                    UpdatedAt = new DateTime(2025, 11, 7)
                },
                // Personal - Wednesday 3PM
                new Event
                {
                    Id = 5,
                    Title = "Dentist Appointment",
                    Description = "Regular dental checkup",
                    StartTime = baseDate.AddDays(3 - (int)baseDate.DayOfWeek).AddHours(15),
                    EndTime = baseDate.AddDays(3 - (int)baseDate.DayOfWeek).AddHours(16),
                    Location = "Dental Clinic",
                    AllDay = false,
                    IsRecurring = false,
                    OwnerId = 2,
                    Visibility = EventVisibility.Private,
                    Status = EventStatus.Confirmed,
                    Color = "#6f42c1",
                    CreatedAt = new DateTime(2025, 11, 27),
                    UpdatedAt = new DateTime(2025, 11, 27)
                },
                // Social - Saturday 6PM
                new Event
                {
                    Id = 6,
                    Title = "Birthday Party - Emma",
                    Description = "Emma's 30th birthday celebration",
                    StartTime = baseDate.AddDays(6 - (int)baseDate.DayOfWeek).AddHours(18),
                    EndTime = baseDate.AddDays(6 - (int)baseDate.DayOfWeek).AddHours(23),
                    Location = "Emma's House",
                    AllDay = false,
                    IsRecurring = false,
                    OwnerId = 2,
                    Visibility = EventVisibility.Shared,
                    Status = EventStatus.Confirmed,
                    Color = "#e83e8c",
                    CreatedAt = new DateTime(2025, 12, 2),
                    UpdatedAt = new DateTime(2025, 12, 2)
                },
                // Business - Thursday 11AM
                new Event
                {
                    Id = 7,
                    Title = "Client Call - Project X",
                    Description = "Discuss project requirements and timeline",
                    StartTime = baseDate.AddDays(4 - (int)baseDate.DayOfWeek).AddHours(11),
                    EndTime = baseDate.AddDays(4 - (int)baseDate.DayOfWeek).AddHours(12),
                    Location = "Virtual - Zoom",
                    AllDay = false,
                    IsRecurring = false,
                    OwnerId = 1,
                    Visibility = EventVisibility.Public,
                    Status = EventStatus.Confirmed,
                    Color = "#007bff",
                    CreatedAt = new DateTime(2025, 12, 5),
                    UpdatedAt = new DateTime(2025, 12, 5)
                },
                // Personal - Sunday 12:30PM
                new Event
                {
                    Id = 8,
                    Title = "Lunch with Mom",
                    Description = "Monthly catch-up lunch",
                    StartTime = baseDate.AddDays(7 - (int)baseDate.DayOfWeek).AddHours(12).AddMinutes(30),
                    EndTime = baseDate.AddDays(7 - (int)baseDate.DayOfWeek).AddHours(14),
                    Location = "Favorite Restaurant",
                    AllDay = false,
                    IsRecurring = true,
                    RecurrenceRule = "FREQ=MONTHLY;BYMONTHDAY=15",
                    OwnerId = 3,
                    Visibility = EventVisibility.Private,
                    Status = EventStatus.Confirmed,
                    Color = "#fd7e14",
                    CreatedAt = new DateTime(2025, 11, 22),
                    UpdatedAt = new DateTime(2025, 11, 22)
                }
            };
        }

        public List<EventAttendee> GenerateEventAttendees()
        {
            return new List<EventAttendee>
            {
                // Alex invites Sarah & Mike to Friday drinks (Event 3)
                new EventAttendee
                {
                    EventId = 3,
                    UserId = 2,
                    Status = AttendeeStatus.Accepted,
                    Role = AttendeeRole.Participant,
                    CreatedAt = new DateTime(2025, 12, 4),
                    UpdatedAt = new DateTime(2025, 12, 5)
                },
                new EventAttendee
                {
                    EventId = 3,
                    UserId = 3,
                    Status = AttendeeStatus.Tentative,
                    Role = AttendeeRole.Participant,
                    CreatedAt = new DateTime(2025, 12, 4),
                    UpdatedAt = new DateTime(2025, 12, 6)
                },
                // Sarah invites Alex to birthday party (Event 6)
                new EventAttendee
                {
                    EventId = 6,
                    UserId = 1,
                    Status = AttendeeStatus.Accepted,
                    Role = AttendeeRole.Participant,
                    CreatedAt = new DateTime(2025, 12, 2),
                    UpdatedAt = new DateTime(2025, 12, 3)
                },
                // Mike invites Sarah to workout (Event 4)
                new EventAttendee
                {
                    EventId = 4,
                    UserId = 2,
                    Status = AttendeeStatus.Invited,
                    Role = AttendeeRole.Participant,
                    CreatedAt = new DateTime(2025, 11, 30),
                    UpdatedAt = new DateTime(2025, 11, 30)
                },
                // Add owners as attendees for their events
                new EventAttendee
                {
                    EventId = 1,
                    UserId = 1,
                    Status = AttendeeStatus.Accepted,
                    Role = AttendeeRole.Owner,
                    CreatedAt = new DateTime(2025, 11, 30),
                    UpdatedAt = new DateTime(2025, 11, 30)
                },
                new EventAttendee
                {
                    EventId = 2,
                    UserId = 2,
                    Status = AttendeeStatus.Accepted,
                    Role = AttendeeRole.Owner,
                    CreatedAt = new DateTime(2025, 11, 23),
                    UpdatedAt = new DateTime(2025, 11, 23)
                }
            };
        }

        public List<Notification> GenerateNotifications()
        {
            return new List<Notification>
            {
                // Event invitation for Sarah to Friday drinks
                new Notification
                {
                    Id = 1,
                    UserId = 2,
                    EventId = 3,
                    Type = NotificationType.EventInvite,
                    Message = "Alex Johnson invited you to 'Friday Night Drinks'",
                    IsRead = true,
                    NotifyAt = new DateTime(2025, 12, 4),
                    CreatedAt = new DateTime(2025, 12, 4),
                    UpdatedAt = new DateTime(2025, 12, 5)
                },
                // Event invitation for Mike to Friday drinks
                new Notification
                {
                    Id = 2,
                    UserId = 3,
                    EventId = 3,
                    Type = NotificationType.EventInvite,
                    Message = "Alex Johnson invited you to 'Friday Night Drinks'",
                    IsRead = false,
                    NotifyAt = new DateTime(2025, 12, 4),
                    CreatedAt = new DateTime(2025, 12, 4),
                    UpdatedAt = new DateTime(2025, 12, 4)
                },
                // Event invitation for Alex to birthday party
                new Notification
                {
                    Id = 3,
                    UserId = 1,
                    EventId = 6,
                    Type = NotificationType.EventInvite,
                    Message = "Sarah Chen invited you to 'Birthday Party - Emma'",
                    IsRead = true,
                    NotifyAt = new DateTime(2025, 12, 2),
                    CreatedAt = new DateTime(2025, 12, 2),
                    UpdatedAt = new DateTime(2025, 12, 3)
                },
                // Reminder for dentist appointment
                new Notification
                {
                    Id = 4,
                    UserId = 2,
                    EventId = 5,
                    Type = NotificationType.Reminder,
                    Message = "Reminder: 'Dentist Appointment' tomorrow at 3:00 PM",
                    IsRead = false,
                    NotifyAt = new DateTime(2025, 12, 6),
                    CreatedAt = new DateTime(2025, 12, 6),
                    UpdatedAt = new DateTime(2025, 12, 6)
                },
                // Event update notification
                new Notification
                {
                    Id = 5,
                    UserId = 1,
                    EventId = 7,
                    Type = NotificationType.EventUpdate,
                    Message = "Event 'Client Call - Project X' has been confirmed",
                    IsRead = true,
                    NotifyAt = new DateTime(2025, 12, 5),
                    CreatedAt = new DateTime(2025, 12, 5),
                    UpdatedAt = new DateTime(2025, 12, 5)
                },
                // Custom notification
                new Notification
                {
                    Id = 6,
                    UserId = 3,
                    Type = NotificationType.Custom,
                    Message = "Don't forget to bring your gym card for tomorrow's workout!",
                    IsRead = false,
                    NotifyAt = new DateTime(2025, 12, 7, 10, 0, 0),
                    CreatedAt = new DateTime(2025, 12, 7, 10, 0, 0),
                    UpdatedAt = new DateTime(2025, 12, 7, 10, 0, 0)
                }
            };
        }
    }
}