using CalendarApp.Domain.Entities;

public interface IEventRepository
{
    Task<Event?> GetByIdAsync(int id);
    Task<IList<Event>> GetAllAsync();
    Task AddAsync(Event @event);
    Task UpdateAsync(Event @event);
    Task DeleteAsync(int id);
}