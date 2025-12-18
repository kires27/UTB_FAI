using CalendarApp.Application.Abstraction;
using CalendarApp.Domain.Entities;
using CalendarApp.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace CalendarApp.Application.Implementation
{
    public class EventAppService : IEventAppService
    {
        private readonly CalendarDbContext _calendarDbContext;

        public EventAppService(CalendarDbContext calendarDbContext)
        {
            _calendarDbContext = calendarDbContext;
        }

        public IList<Event> SelectAll()
        {
            return _calendarDbContext.Events
                .Include(e => e.Owner)
                .Include(e => e.EventAttendees)
                .ThenInclude(ea => ea.User)
                .ToList();
        }

        public Event? GetById(int id)
        {
            return _calendarDbContext.Events
                .Include(e => e.Owner)
                .Include(e => e.EventAttendees)
                .ThenInclude(ea => ea.User)
                .FirstOrDefault(e => e.Id == id);
        }

        public void Create(Event @event)
        {
            _calendarDbContext.Events.Add(@event);
            _calendarDbContext.SaveChanges();

            // Automatically add event owner as attendee with Owner role
            var ownerAttendee = new EventAttendee
            {
                EventId = @event.Id,
                UserId = @event.OwnerId,
                Role = AttendeeRole.Owner,
                Status = AttendeeStatus.Accepted,
                UpdatedAt = DateTime.UtcNow
            };
            
            _calendarDbContext.EventAttendees.Add(ownerAttendee);
            _calendarDbContext.SaveChanges();
        }

        public bool Delete(int id)
        {
            var eventToDelete = _calendarDbContext.Events
                .Include(e => e.EventAttendees)
                .FirstOrDefault(e => e.Id == id);
                
            if (eventToDelete != null)
            {
                // EventAttendees will be deleted automatically due to cascade delete
                _calendarDbContext.Events.Remove(eventToDelete);
                _calendarDbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public void Update(Event @event)
        {
            var existingEvent = _calendarDbContext.Events.Find(@event.Id);
            if (existingEvent != null)
            {
                // Update only the modifiable properties
                existingEvent.Title = @event.Title;
                existingEvent.Description = @event.Description;
                existingEvent.StartTime = @event.StartTime;
                existingEvent.EndTime = @event.EndTime;
                existingEvent.Location = @event.Location;
                existingEvent.Status = @event.Status;
                existingEvent.AllDay = @event.AllDay;
                existingEvent.IsRecurring = @event.IsRecurring;
                existingEvent.RecurrenceRule = @event.RecurrenceRule;
                existingEvent.RecurrenceEnd = @event.RecurrenceEnd;
                existingEvent.ReminderTime = @event.ReminderTime;
                existingEvent.Color = @event.Color;
                existingEvent.UpdatedAt = DateTime.UtcNow;
                
                _calendarDbContext.SaveChanges();
            }
        }

        public async Task<bool> InviteUsersToEvent(int eventId, List<int> userIds)
        {
            var @event = await _calendarDbContext.Events
                .Include(e => e.Owner)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (@event == null)
                return false;

            foreach (var userId in userIds)
            {
                // Check if user is already an attendee
                var existingAttendee = await _calendarDbContext.EventAttendees
                    .FirstOrDefaultAsync(ea => ea.EventId == eventId && ea.UserId == userId);

                if (existingAttendee == null)
                {
                    // Add user as attendee with Invited status
                    var attendee = new EventAttendee
                    {
                        EventId = eventId,
                        UserId = userId,
                        Status = AttendeeStatus.Invited,
                        Role = AttendeeRole.Participant,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _calendarDbContext.EventAttendees.Add(attendee);

                    // Create notification for the invited user
                    var notification = new Notification
                    {
                        UserId = userId,
                        EventId = eventId,
                        Type = NotificationType.EventInvite,
                        Message = $"{@event.Owner?.UserName ?? "Someone"} invited you to '{@event.Title}'",
                        IsRead = false,
                        NotifyAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _calendarDbContext.Notifications.Add(notification);
                }
            }

            await _calendarDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<User>> FindUsersByEmailsAsync(List<string> emails)
        {
            return await _calendarDbContext.Users
                .Where(u => u.Email != null && emails.Contains(u.Email))
                .ToListAsync();
        }

        public async Task<bool> RemoveAttendeeFromEvent(int eventId, int userId)
        {
            var attendee = await _calendarDbContext.EventAttendees
                .FirstOrDefaultAsync(ea => ea.EventId == eventId && ea.UserId == userId);

            if (attendee != null && attendee.Role != AttendeeRole.Owner)
            {
                _calendarDbContext.EventAttendees.Remove(attendee);
                await _calendarDbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}