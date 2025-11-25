using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTB.eshop25.Application.Abstraction;
using UTB.eshop25.Domain.Entities;
using UTB.eshop25.Infrastructure.Database;

namespace UTB.eshop25.Application.Implementation
{
    public class ProductAppService : IProductAppService
    {
        EshopDbContext _dbContext;
        public ProductAppService(EshopDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public IList<Product> SelectAll()
        {
            return _dbContext.Products.ToList();
        }

        public void Create(Product product)
        {
            _dbContext.Products.Add(product);
            _dbContext.SaveChanges();
        }

        public Product? Select(int id)
        {
            return _dbContext.Products.FirstOrDefault(product => product.Id == id);
        }

        public bool Delete(int id)
        {
            Product? product = Select(id);
            if (product != null)
            {
                _dbContext.Products.Remove(product);
                _dbContext.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
