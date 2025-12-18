using CalendarApp.Domain.Entities;

namespace CalendarApp.Infrastructure.Database.Seeding
{
	public class EventAttendeesInit
	{
		public List<EventAttendee> GenerateEventAttendees()
		{
			List<EventAttendee> entities = new List<EventAttendee>();

			var entity1 = new EventAttendee()
			{
				EventId = -6, // Friday Night Drinks
				UserId = -2, // Sarah
				Status = AttendeeStatus.Accepted,
				Role = AttendeeRole.Participant,
				UpdatedAt = new DateTime(2025, 12, 5)
			};

			var entity2 = new EventAttendee()
			{
				EventId = -6, // Friday Night Drinks
				UserId = -3, // Mike
				Status = AttendeeStatus.Tentative,
				Role = AttendeeRole.Participant,
				UpdatedAt = new DateTime(2025, 12, 6)
			};

			var entity3 = new EventAttendee()
			{
				EventId = -7, // Birthday Party
				UserId = -1, // Alex
				Status = AttendeeStatus.Accepted,
				Role = AttendeeRole.Participant,
				UpdatedAt = new DateTime(2025, 12, 3)
			};

			var entity4 = new EventAttendee()
			{
				EventId = -5, // Morning Workout
				UserId = -2, // Sarah
				Status = AttendeeStatus.Invited,
				Role = AttendeeRole.Participant,
				UpdatedAt = new DateTime(2025, 11, 30)
			};

			var entity5 = new EventAttendee()
			{
				EventId = -1, // Q4 Planning
				UserId = -1, // Alex
				Status = AttendeeStatus.Accepted,
				Role = AttendeeRole.Owner,
				UpdatedAt = new DateTime(2025, 11, 30)
			};

			var entity6 = new EventAttendee()
			{
				EventId = -3, // Grocery Shopping
				UserId = -2, // Sarah
				Status = AttendeeStatus.Accepted,
				Role = AttendeeRole.Owner,
				UpdatedAt = new DateTime(2025, 11, 23)
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
