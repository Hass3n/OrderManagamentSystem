using OrderManagementSystem.Application.DTOs;

namespace OrderManagementSystem.Application.Services
{
    public interface ICustomerService
    {
        Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto createCustomerDto);
        Task<CustomerDto?> GetCustomerByIdAsync(int customerId);
        Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();
        Task<IEnumerable<OrderDto>> GetCustomerOrdersAsync(int customerId);
        Task<bool> CustomerExistsAsync(int customerId);
    }
}

