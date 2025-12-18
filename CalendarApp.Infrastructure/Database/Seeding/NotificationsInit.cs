using CalendarApp.Domain.Entities;

namespace CalendarApp.Infrastructure.Database.Seeding
{
	public class NotificationsInit
	{
		public List<Notification> GenerateNotifications()
		{
			List<Notification> entities = new List<Notification>();

			var entity1 = new Notification()
			{
				Id = -1,
				UserId = -2, // Sarah
				EventId = -6, // Friday Night Drinks
				Type = NotificationType.EventInvite,
				Message = "Alex Johnson invited you to 'Friday Night Drinks'",
				IsRead = true,
				NotifyAt = new DateTime(2025, 12, 4),
				UpdatedAt = new DateTime(2025, 12, 5)
			};

			// Event invitation for Mike to Friday drinks
			var entity2 = new Notification()
			{
				Id = -2,
				UserId = -3, // Mike
				EventId = -6, // Friday Night Drinks
				Type = NotificationType.EventInvite,
				Message = "Alex Johnson invited you to 'Friday Night Drinks'",
				IsRead = false,
				NotifyAt = new DateTime(2025, 12, 4),
				UpdatedAt = new DateTime(2025, 12, 4)
			};

			// Event invitation for Alex to birthday party
			var entity3 = new Notification()
			{
				Id = -3,
				UserId = -1, // Alex
				EventId = -7, // Birthday Party
				Type = NotificationType.EventInvite,
				Message = "Sarah Chen invited you to 'Birthday Party - Emma'",
				IsRead = true,
				NotifyAt = new DateTime(2025, 12, 2),
				UpdatedAt = new DateTime(2025, 12, 3)
			};

			// Reminder for dentist appointment
			var entity4 = new Notification()
			{
				Id = -4,
				UserId = -2, // Sarah
				EventId = -4, // Dentist Appointment
				Type = NotificationType.Reminder,
				Message = "Reminder: 'Dentist Appointment' tomorrow at 3:00 PM",
				IsRead = false,
				NotifyAt = new DateTime(2025, 12, 14),
				UpdatedAt = new DateTime(2025, 12, 13)
			};

			// Event update notification
			var entity5 = new Notification()
			{
				Id = -5,
				UserId = -1, // Alex
				EventId = -2, // Client Call
				Type = NotificationType.EventUpdate,
				Message = "Event 'Client Call - Project X' has been confirmed",
				IsRead = true,
				NotifyAt = new DateTime(2025, 12, 5),
				UpdatedAt = new DateTime(2025, 12, 5)
			};

			// Custom notification
			var entity6 = new Notification()
			{
				Id = -6,
				UserId = -3, // Mike
				Type = NotificationType.Custom,
				Message = "Don't forget to bring your gym card for tomorrow's workout!",
				IsRead = false,
				NotifyAt = new DateTime(2025, 12, 15, 10, 0, 0),
				UpdatedAt = new DateTime(2025, 12, 15, 10, 0, 0)
			};

			entities.Add(entity1);
			entities.Add(entity2);
			entities.Add(entity3);
			entities.Add(entity4);
			entities.Add(entity5);
			entities.Add(entity6);

			return entities;
		}

	}

}
