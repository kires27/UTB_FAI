using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTB.eshop25.Domain.Entities.Interface;

namespace UTB.eshop25.Domain.Entities
{
    public abstract class Entity<T> : IEntity<T>
    {
        public T Id { get; set; }
    }
}
