using OrderManagementSystem.Application.DTOs;

namespace OrderManagementSystem.Application.Services
{
    public interface IInvoiceService
    {
        Task<InvoiceDto> CreateInvoiceAsync(int orderId);
        Task<InvoiceDto?> GetInvoiceByIdAsync(int invoiceId);
        Task<IEnumerable<InvoiceDto>> GetAllInvoicesAsync();
        Task<InvoiceDto?> GetInvoiceByOrderIdAsync(int orderId);
    }
}

