using CalendarApp.Domain.Entities;

namespace CalendarApp.Application.Abstraction
{
    public interface IEventAppService
    {
        IList<Event> SelectAll();
        Event? GetById(int id);
        void Create(Event @event);
        bool Delete(int id);
        void Update(Event @event);
    }
}