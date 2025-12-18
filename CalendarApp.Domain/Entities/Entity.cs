using CalendarApp.Domain.Entities.Interface;

namespace CalendarApp.Domain.Entities
{
    public abstract class Entity<T> : IEntity<T>
    {
		public T Id { get; set; } = default!;
    }
}
