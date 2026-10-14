using CalendarApp.Application.Abstraction;
using CalendarApp.Domain.Entities;
using CalendarApp.Infrastructure.Identity;
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
                .Include(e => e.EventAttendees)
                .Include(e => e.Exceptions)
                .ToList();
        }

        public IList<Event> SelectByOwner(int ownerId)
        {
            return _calendarDbContext.Events
                .Include(e => e.EventAttendees)
                .Include(e => e.Exceptions)
                .Where(e => e.OwnerId == ownerId)
                .ToList();
        }

        public Event? GetById(int id)
        {
            return _calendarDbContext.Events
                .Include(e => e.EventAttendees)
                .Include(e => e.Exceptions)
                .FirstOrDefault(e => e.Id == id);
        }

        public EventOccurrenceException? GetOccurrenceException(int masterEventId, DateTime originalDate)
        {
            return _calendarDbContext.EventOccurrenceExceptions
                .FirstOrDefault(x => x.MasterEventId == masterEventId && x.OriginalDate == originalDate.Date);
        }

        public void UpsertOccurrenceException(EventOccurrenceException exception)
        {
            var existing = _calendarDbContext.EventOccurrenceExceptions
                .FirstOrDefault(x => x.MasterEventId == exception.MasterEventId && x.OriginalDate == exception.OriginalDate.Date);

            if (existing == null)
            {
                exception.OriginalDate = exception.OriginalDate.Date;
                _calendarDbContext.EventOccurrenceExceptions.Add(exception);
            }
            else
            {
                existing.IsCancelled = exception.IsCancelled;
                existing.Title = exception.Title;
                existing.Description = exception.Description;
                existing.StartTime = exception.StartTime;
                existing.EndTime = exception.EndTime;
                existing.AllDay = exception.AllDay;
                existing.Location = exception.Location;
                existing.ReminderTime = exception.ReminderTime;
                existing.Color = exception.Color;
                existing.Status = exception.Status;
                existing.UpdatedAt = DateTime.UtcNow;
            }

            _calendarDbContext.SaveChanges();
        }

        /// <summary>
        /// Moves an event to a new start/end (drag & drop in the week view).
        /// Non-recurring events are updated directly; for recurring events only the
        /// single occurrence identified by <paramref name="occurrenceDate"/> is moved
        /// via an exception row (all other occurrences keep their original schedule).
        /// Returns false when the event (or the occurrence slot) does not exist.
        /// </summary>
        public bool MoveOccurrence(int eventId, DateTime? occurrenceDate, DateTime newStart, DateTime newEnd, bool allDay)
        {
            var @event = _calendarDbContext.Events.FirstOrDefault(e => e.Id == eventId);
            if (@event == null)
            {
                return false;
            }

            // Business backstop: end must not be before start; only non-all-day
            // events may start and end at the exact same instant (zero-length).
            if (newEnd < newStart || (!allDay && newEnd == newStart))
            {
                throw new ArgumentException("End time must be after start time.");
            }

            if (@event.IsRecurring)
            {
                if (!occurrenceDate.HasValue)
                {
                    return false;
                }

                // Preserve text/color overrides a previous per-occurrence edit may
                // have stored — a drag only changes the schedule fields.
                var existing = GetOccurrenceException(eventId, occurrenceDate.Value.Date);

                UpsertOccurrenceException(new EventOccurrenceException
                {
                    MasterEventId = eventId,
                    OriginalDate = occurrenceDate.Value.Date,
                    IsCancelled = false,
                    Title = existing?.Title,
                    Description = existing?.Description,
                    StartTime = newStart,
                    EndTime = newEnd,
                    AllDay = allDay,
                    Location = existing?.Location,
                    ReminderTime = existing?.ReminderTime,
                    Color = existing?.Color,
                    Status = existing?.Status,
                    CreatedAt = existing?.CreatedAt ?? DateTime.Now,
                    UpdatedAt = DateTime.Now
                });
            }
            else
            {
                @event.StartTime = newStart;
                @event.EndTime = newEnd;
                @event.AllDay = allDay;
                @event.UpdatedAt = DateTime.UtcNow;
                _calendarDbContext.SaveChanges();
            }

            return true;
        }

        public void Create(Event @event)
        {
            ValidateEventBusinessRules(@event);

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
                .Include(e => e.Notifications)
                .Include(e => e.Exceptions)
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
            ValidateEventBusinessRules(@event);

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

        /// <summary>
        /// Business-level backstop for event time rules, behind the ViewModel validation.
        /// Throws ArgumentException so callers (controllers) can surface a model error.
        /// </summary>
        private static void ValidateEventBusinessRules(Event @event)
        {
            // End must not be before start; only non-all-day events may start and end at the
            // exact same instant (zero-length).
            if (@event.EndTime < @event.StartTime || (!@event.AllDay && @event.EndTime == @event.StartTime))
            {
                throw new ArgumentException("End time must be after start time for a non-all-day event.");
            }

            // Recurrence may not end before the day the series starts.
            if (@event.RecurrenceEnd.HasValue && @event.RecurrenceEnd.Value < @event.StartTime.Date)
            {
                throw new ArgumentException("Recurrence end cannot be before the event start.");
            }
        }

        public async Task<bool> InviteUsersToEvent(int eventId, List<int> userIds)
        {
            var @event = await _calendarDbContext.Events
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (@event == null)
                return false;

            var owner = await _calendarDbContext.Users.FindAsync(@event.OwnerId);

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
                        Message = $"{owner?.UserName ?? "Someone"} invited you to '{@event.Title}'",
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

        public Dictionary<int, User> GetUsersByIds(IEnumerable<int> userIds)
        {
            var ids = userIds.Distinct().ToList();
            return _calendarDbContext.Users
                .Where(u => ids.Contains(u.Id))
                .ToDictionary(u => u.Id);
        }

        public async Task<List<User>> FindUsersByEmailsAsync(List<string> emails)
        {
            return await _calendarDbContext.Users
                .Where(u => u.Email != null && emails.Contains(u.Email))
                .ToListAsync();
        }

        public async Task<List<User>> SearchUsersAsync(string term)
        {
            return await _calendarDbContext.Users
                .Where(u => (u.UserName != null && u.UserName.Contains(term)) ||
                            (u.Email != null && u.Email.Contains(term)))
                .OrderBy(u => u.UserName)
                .Take(10)
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

        public async Task<bool> DeclineEventAsync(int eventId, int userId)
        {
            var attendee = await _calendarDbContext.EventAttendees
                .Include(ea => ea.Event)
                .FirstOrDefaultAsync(ea => ea.EventId == eventId && ea.UserId == userId);

            if (attendee == null || attendee.Role == AttendeeRole.Owner || attendee.Status == AttendeeStatus.Declined)
            {
                return false;
            }

            attendee.Status = AttendeeStatus.Declined;
            attendee.UpdatedAt = DateTime.UtcNow;

            var invites = await _calendarDbContext.Notifications
                .Where(n => n.UserId == userId && n.EventId == eventId && n.Type == NotificationType.EventInvite && !n.IsRead)
                .ToListAsync();
            foreach (var invite in invites)
            {
                invite.IsRead = true;
                invite.UpdatedAt = DateTime.UtcNow;
            }

            var decliner = await _calendarDbContext.Users.FindAsync(userId);

            _calendarDbContext.Notifications.Add(new Notification
            {
                UserId = attendee.Event.OwnerId,
                EventId = eventId,
                Type = NotificationType.EventUpdate,
                Message = $"{decliner?.UserName ?? "Someone"} declined '{attendee.Event.Title}'",
                IsRead = false,
                NotifyAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            await _calendarDbContext.SaveChangesAsync();
            return true;
        }
    }
}