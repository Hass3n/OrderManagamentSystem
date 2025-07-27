using OrderManagementSystem.Application.DTOs;

namespace OrderManagementSystem.Application.Services
{
    public interface IProductService
    {
        Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto);
        Task<ProductDto?> GetProductByIdAsync(int productId);
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto> UpdateProductAsync(int productId, UpdateProductDto updateProductDto);
        Task DeleteProductAsync(int productId);
        Task<bool> ProductExistsAsync(int productId);
        Task<bool> IsStockAvailableAsync(int productId, int quantity);
    }
}

