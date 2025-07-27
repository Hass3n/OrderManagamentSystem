using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Domain.Entities;
using OrderManagementSystem.Domain.Interfaces;
using OrderManagementSystem.Infrastructure.Data;

namespace OrderManagementSystem.Infrastructure.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(OrderManagementDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetProductsByIdsAsync(IEnumerable<int> productIds)
        {
            return await _dbSet.Where(p => productIds.Contains(p.ProductId)).ToListAsync();
        }

        public async Task UpdateStockAsync(int productId, int quantity)
        {
            var product = await _dbSet.FindAsync(productId);
            if (product != null)
            {
                product.Stock -= quantity;
                _dbSet.Update(product);
            }
        }

        public async Task<bool> IsStockAvailableAsync(int productId, int quantity)
        {
            var product = await _dbSet.FindAsync(productId);
            return product != null && product.Stock >= quantity;
        }
    }
}

