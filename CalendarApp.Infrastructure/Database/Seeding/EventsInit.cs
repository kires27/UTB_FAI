using CalendarApp.Domain.Entities;

namespace CalendarApp.Infrastructure.Database.Seeding
{
	public class EventsInit
	{
		public List<Event> GenerateEvents()
		{
            var baseDate = new DateTime(2025, 12, 15, 0, 0, 0); // Use current date for seeding

			List<Event> entities = new List<Event>();

			var entity1 = new Event()
			{
				Id = -1,
				Title = "Q4 Planning Session",
				Description = "Quarterly business planning and strategy meeting",
				StartTime = baseDate.AddDays(-(int)baseDate.DayOfWeek + 1).AddHours(9), // Monday 9AM
				EndTime = baseDate.AddDays(-(int)baseDate.DayOfWeek + 1).AddHours(11),
				Location = "Conference Room A",
				AllDay = false,
				IsRecurring = false,
				OwnerId = -1, // Alex
				Status = EventStatus.Confirmed,
				Color = "#007bff",
				UpdatedAt = new DateTime(2025, 11, 30)
			};

			var entity2 = new Event()
			{
				Id = -2,
				Title = "Client Call - Project X",
				Description = "Discuss project requirements and timeline",
				StartTime = baseDate.AddDays(4 - (int)baseDate.DayOfWeek).AddHours(11), // Thursday 11AM
				EndTime = baseDate.AddDays(4 - (int)baseDate.DayOfWeek).AddHours(12),
				Location = "Virtual - Zoom",
				AllDay = false,
				IsRecurring = false,
				OwnerId = -1, // Alex
				Status = EventStatus.Confirmed,
				Color = "#007bff",
				UpdatedAt = new DateTime(2025, 12, 5)
			};

			var entity3 = new Event()
			{
				Id = -3,
				Title = "Grocery Shopping",
				Description = "Weekly grocery run",
				StartTime = baseDate.AddDays(6 - (int)baseDate.DayOfWeek).AddHours(14), // Saturday 2PM
				EndTime = baseDate.AddDays(6 - (int)baseDate.DayOfWeek).AddHours(15),
				Location = "Supermarket",
				AllDay = false,
				IsRecurring = true,
				RecurrenceRule = "FREQ=WEEKLY;BYDAY=SA",
				OwnerId = -2, // Sarah
				Status = EventStatus.Confirmed,
				Color = "#28a745",
				UpdatedAt = new DateTime(2025, 11, 23)
			};

			var entity4 = new Event()
			{
				Id = -4,
				Title = "Dentist Appointment",
				Description = "Regular dental checkup",
				StartTime = baseDate.AddDays(3 - (int)baseDate.DayOfWeek).AddHours(15), // Wednesday 3PM
				EndTime = baseDate.AddDays(3 - (int)baseDate.DayOfWeek).AddHours(16),
				Location = "Dental Clinic",
				AllDay = false,
				IsRecurring = false,
				OwnerId = -2, // Sarah
				Status = EventStatus.Confirmed,
				Color = "#6f42c1",
				UpdatedAt = new DateTime(2025, 11, 27)
			};

			var entity5 = new Event()
			{
				Id = -5,
				Title = "Morning Workout",
				Description = "Cardio and strength training",
				StartTime = baseDate.AddDays(2 - (int)baseDate.DayOfWeek).AddHours(6).AddMinutes(30), // Tuesday 6:30AM
				EndTime = baseDate.AddDays(2 - (int)baseDate.DayOfWeek).AddHours(7).AddMinutes(30),
				Location = "Gym",
				AllDay = false,
				IsRecurring = true,
				RecurrenceRule = "FREQ=WEEKLY;BYDAY=TU,TH",
				OwnerId = -3, // Mike
				Status = EventStatus.Confirmed,
				Color = "#dc3545",
				UpdatedAt = new DateTime(2025, 11, 7)
			};
			var entity6 = new Event()
			{
				Id = -6,
				Title = "Friday Night Drinks",
				Description = "Team happy hour after work",
				StartTime = baseDate.AddDays(5 - (int)baseDate.DayOfWeek).AddHours(19), // Friday 7PM
				EndTime = baseDate.AddDays(5 - (int)baseDate.DayOfWeek).AddHours(22),
				Location = "The Local Pub",
				AllDay = false,
				IsRecurring = false,
				OwnerId = -1, // Alex
				Status = EventStatus.Confirmed,
				Color = "#ffc107",
				UpdatedAt = new DateTime(2025, 12, 4)
			};

			var entity7 = new Event()
			{
				Id = -7,
				Title = "Birthday Party - Emma",
				Description = "Emma's 30th birthday celebration",
				StartTime = baseDate.AddDays(6 - (int)baseDate.DayOfWeek).AddHours(18), // Saturday 6PM
				EndTime = baseDate.AddDays(6 - (int)baseDate.DayOfWeek).AddHours(23),
				Location = "Emma's House",
				AllDay = false,
				IsRecurring = false,
				OwnerId = -2, // Sarah
				Status = EventStatus.Confirmed,
				Color = "#e83e8c",
				UpdatedAt = new DateTime(2025, 12, 2)
			};

			var entity8 = new Event()
			{
				Id = -8,
				Title = "Lunch with Mom",
				Description = "Monthly catch-up lunch",
				StartTime = baseDate.AddDays(7 - (int)baseDate.DayOfWeek).AddHours(12).AddMinutes(30), // Sunday 12:30PM
				EndTime = baseDate.AddDays(7 - (int)baseDate.DayOfWeek).AddHours(14),
				Location = "Favorite Restaurant",
				AllDay = false,
				IsRecurring = true,
				RecurrenceRule = "FREQ=MONTHLY;BYMONTHDAY=15",
				OwnerId = -3, // Mike
				Status = EventStatus.Confirmed,
				Color = "#fd7e14",
				UpdatedAt = new DateTime(2025, 11, 22)
			};

			entities.Add(entity1);
			entities.Add(entity2);
			entities.Add(entity3);
			entities.Add(entity4);
			entities.Add(entity5);
			entities.Add(entity6);
			entities.Add(entity7);
			entities.Add(entity8);

			return entities;
		}
	}
}
