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
        }

        public bool Delete(int id)
        {
            var eventToDelete = _calendarDbContext.Events.Find(id);
            if (eventToDelete != null)
            {
                _calendarDbContext.Events.Remove(eventToDelete);
                _calendarDbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public void Update(Event @event)
        {
            _calendarDbContext.Events.Update(@event);
            _calendarDbContext.SaveChanges();
        }
    }
}