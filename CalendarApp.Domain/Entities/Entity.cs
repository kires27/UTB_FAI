using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalendarApp.Domain.Entities.Interface;

namespace CalendarApp.Domain.Entities
{
    public abstract class Entity<T> : IEntity<T>
    {
        public T Id { get; set; } = default!;
    }
}
