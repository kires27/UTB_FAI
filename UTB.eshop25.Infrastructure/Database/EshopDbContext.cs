using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTB.eshop25.Domain.Entities;
using UTB.eshop25.Infrastructure.Database.Seeding;

namespace UTB.eshop25.Infrastructure.Database
{
    public class EshopDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public EshopDbContext(DbContextOptions dbContextOptions)
            : base(dbContextOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var prodInit = new ProductInit();
            modelBuilder.Entity<Product>()
                        .HasData(prodInit.GenerateProducts3());
        }
    }
}
