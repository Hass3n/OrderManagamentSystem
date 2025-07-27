namespace OrderManagementSystem.Domain.Interfaces
{
    public interface IEmailService
    {
        Task SendOrderConfirmationAsync(string customerEmail, int orderId);
        Task SendOrderStatusUpdateAsync(string customerEmail, int orderId, string status);
        Task SendInvoiceAsync(string customerEmail, int invoiceId);
    }
}

