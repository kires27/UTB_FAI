namespace CalendarApp.Domain.Entities.Interface
{
    public interface IEntity<T>
    {
        public T Id { get; set; }
    }
}
