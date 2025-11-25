using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTB.eshop25.Domain.Entities;

namespace UTB.eshop25.Infrastructure.Database.Seeding
{
    internal class ProductInit
    {
        public List<Product> GenerateProducts3()
        {
            List<Product> products = new List<Product>();

            var product1 = new Product()
            {
                Id = 1,
                Name = "Chleba",
                Description = "Nejlepší chleba na kontinentu",
                ImageSrc = "/img/products/product-1.png",
                Price = 50
            };

            var product2 = new Product()
            {
                Id = 2,
                Name = "Rohlík",
                Description = "Nejlepší rohlík v galaxii",
                ImageSrc = "/img/products/product-2.png",
                Price = 4
            };

            var product3 = new Product()
            {
                Id = 3,
                Name = "Bageta",
                Description = "Nejlepší bageta ve vesmíru",
                ImageSrc = "/img/products/product-3.png",
                Price = 25
            };

            products.Add(product1);
            products.Add(product2);
            products.Add(product3);

            return products;
        }
    }
}
