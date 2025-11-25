using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTB.eshop25.Domain.Validations;

namespace UTB.eshop25.Domain.Entities
{
    [Table(nameof(Product))]
    public class Product : Entity<int>
    {
        [Required]
        [StringLength(70)]
        public string? Name { get; set; }
        [UpperCase]
        public string? Description { get; set; }
        [Range(1, double.MaxValue)]
        public double Price { get; set; }
        public string? ImageSrc { get; set; }
    }
}
