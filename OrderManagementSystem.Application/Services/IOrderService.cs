using OrderManagementSystem.Application.DTOs;
using OrderManagementSystem.Domain.Enums;

namespace OrderManagementSystem.Application.Services
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto);
        Task<OrderDto?> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<IEnumerable<OrderDto>> GetOrdersByCustomerIdAsync(int customerId);
        Task<OrderDto> UpdateOrderStatusAsync(int orderId, OrderStatus status);
        Task<bool> OrderExistsAsync(int orderId);
    }
}

