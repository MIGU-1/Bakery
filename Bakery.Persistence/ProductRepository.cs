using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bakery.Core.Contracts;
using Bakery.Core.DTOs;
using Bakery.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bakery.Persistence
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ProductRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> GetCountAsync()
        {
            return await _dbContext.Products.CountAsync();
        }

        public async Task AddRangeAsync(IEnumerable<Product> products)
        {
            await _dbContext.Products.AddRangeAsync(products);
        }

        public Task<Product[]> GetAllAsync()
            => _dbContext
            .Products
            .Include(p => p.OrderItems)
            .ToArrayAsync();

        public void Update(Product product)
            => _dbContext.Products.Update(product);

        public async Task<Product> GetByIdAsync(int id)
            => await _dbContext
            .Products
            .Where(p => p.Id == id)
            .SingleOrDefaultAsync();

        public async Task AddAsync(Product product)
            => await _dbContext.Products.AddAsync(product);
    }
}
