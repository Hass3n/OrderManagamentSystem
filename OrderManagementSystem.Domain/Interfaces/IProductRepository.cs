using OrderManagementSystem.Domain.Entities;

namespace OrderManagementSystem.Domain.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductsByIdsAsync(IEnumerable<int> productIds);
        Task UpdateStockAsync(int productId, int quantity);
        Task<bool> IsStockAvailableAsync(int productId, int quantity);
    }
}

