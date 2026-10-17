using CalendarApp.Domain.Entities;
using CalendarApp.Domain.Repositories;
using CalendarApp.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace CalendarApp.Infrastructure.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly CalendarDbContext _calendarDbContext;

        public EventRepository(CalendarDbContext calendarDbContext)
        {
            _calendarDbContext = calendarDbContext;
        }

        public async Task<IList<Event>> GetAllAsync()
        {
            return await _calendarDbContext.Events
                .Include(e => e.EventAttendees)
                .Include(e => e.Exceptions)
                .ToListAsync();
        }

        public async Task<IList<Event>> GetByOwnerAsync(int ownerId)
        {
            return await _calendarDbContext.Events
                .Include(e => e.EventAttendees)
                .Include(e => e.Exceptions)
                .Where(e => e.OwnerId == ownerId)
                .ToListAsync();
        }

        public Task<Event?> GetByIdAsync(int id)
        {
            return _calendarDbContext.Events
                .Include(e => e.EventAttendees)
                .Include(e => e.Notifications)
                .Include(e => e.Exceptions)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Event> AddAsync(Event @event)
        {
            _calendarDbContext.Events.Add(@event);
            await _calendarDbContext.SaveChangesAsync();
            return @event;
        }

        public async Task<bool> UpdateAsync(Event @event)
        {
            var existingEvent = await _calendarDbContext.Events.FindAsync(@event.Id);
            if (existingEvent == null)
            {
                return false;
            }

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

            await _calendarDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateScheduleAsync(int eventId, DateTime startTime, DateTime endTime, bool allDay)
        {
            var @event = await _calendarDbContext.Events.FirstOrDefaultAsync(e => e.Id == eventId);
            if (@event == null)
            {
                return false;
            }

            @event.StartTime = startTime;
            @event.EndTime = endTime;
            @event.AllDay = allDay;
            @event.UpdatedAt = DateTime.UtcNow;

            await _calendarDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var eventToDelete = await _calendarDbContext.Events
                .Include(e => e.EventAttendees)
                .Include(e => e.Notifications)
                .Include(e => e.Exceptions)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventToDelete == null)
            {
                return false;
            }

            // Children are loaded so MySQL FK constraints are satisfied on delete
            _calendarDbContext.Events.Remove(eventToDelete);
            await _calendarDbContext.SaveChangesAsync();
            return true;
        }

        public Task<EventAttendee?> GetAttendeeAsync(int eventId, int userId, bool includeEvent = false)
        {
            var query = _calendarDbContext.EventAttendees.AsQueryable();
            if (includeEvent)
            {
                query = query.Include(ea => ea.Event);
            }

            return query.FirstOrDefaultAsync(ea => ea.EventId == eventId && ea.UserId == userId);
        }

        public Task<bool> IsAttendeeAsync(int eventId, int userId)
        {
            return _calendarDbContext.EventAttendees
                .AnyAsync(ea => ea.EventId == eventId && ea.UserId == userId);
        }

        public async Task AddAttendeeAsync(EventAttendee attendee)
        {
            _calendarDbContext.EventAttendees.Add(attendee);
            await _calendarDbContext.SaveChangesAsync();
        }

        public async Task<bool> RemoveAttendeeAsync(int eventId, int userId)
        {
            var attendee = await _calendarDbContext.EventAttendees
                .FirstOrDefaultAsync(ea => ea.EventId == eventId && ea.UserId == userId);

            if (attendee == null)
            {
                return false;
            }

            _calendarDbContext.EventAttendees.Remove(attendee);
            await _calendarDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetAttendeeStatusAsync(int eventId, int userId, AttendeeStatus status)
        {
            var attendee = await _calendarDbContext.EventAttendees
                .FirstOrDefaultAsync(ea => ea.EventId == eventId && ea.UserId == userId);

            if (attendee == null)
            {
                return false;
            }

            attendee.Status = status;
            attendee.UpdatedAt = DateTime.UtcNow;
            await _calendarDbContext.SaveChangesAsync();
            return true;
        }

        public Task<EventOccurrenceException?> GetOccurrenceExceptionAsync(int masterEventId, DateTime originalDate)
        {
            return _calendarDbContext.EventOccurrenceExceptions
                .FirstOrDefaultAsync(x => x.MasterEventId == masterEventId && x.OriginalDate == originalDate.Date);
        }

        public async Task UpsertOccurrenceExceptionAsync(EventOccurrenceException exception)
        {
            var existing = await _calendarDbContext.EventOccurrenceExceptions
                .FirstOrDefaultAsync(x => x.MasterEventId == exception.MasterEventId && x.OriginalDate == exception.OriginalDate.Date);

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

            await _calendarDbContext.SaveChangesAsync();
        }
    }
}
