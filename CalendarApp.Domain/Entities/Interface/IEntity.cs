using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarApp.Domain.Entities.Interface
{
    public interface IEntity<T>
    {
        public T Id { get; set; }
    }
}
