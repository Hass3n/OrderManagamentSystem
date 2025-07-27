using AutoMapper;
using OrderManagementSystem.Application.DTOs;
using OrderManagementSystem.Domain.Entities;
using OrderManagementSystem.Domain.Interfaces;

namespace OrderManagementSystem.Application.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public InvoiceService(IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailService = emailService;
        }

        public async Task<InvoiceDto> CreateInvoiceAsync(int orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new ArgumentException($"Order with ID {orderId} not found");
            }

            // Check if invoice already exists for this order
            var existingInvoice = await _unitOfWork.Invoices.GetByOrderIdAsync(orderId);
            if (existingInvoice != null)
            {
                return _mapper.Map<InvoiceDto>(existingInvoice);
            }

            var invoice = new Invoice
            {
                OrderId = orderId,
                InvoiceDate = DateTime.UtcNow,
                TotalAmount = order.TotalAmount
            };

            await _unitOfWork.Invoices.AddAsync(invoice);
            await _unitOfWork.SaveChangesAsync();

            // Send invoice email
            await _emailService.SendInvoiceAsync(order.Customer.Email, invoice.InvoiceId);

            return _mapper.Map<InvoiceDto>(invoice);
        }

        public async Task<InvoiceDto?> GetInvoiceByIdAsync(int invoiceId)
        {
            var invoice = await _unitOfWork.Invoices.GetByIdAsync(invoiceId);
            return invoice != null ? _mapper.Map<InvoiceDto>(invoice) : null;
        }

        public async Task<IEnumerable<InvoiceDto>> GetAllInvoicesAsync()
        {
            var invoices = await _unitOfWork.Invoices.GetAllAsync();
            return _mapper.Map<IEnumerable<InvoiceDto>>(invoices);
        }

        public async Task<InvoiceDto?> GetInvoiceByOrderIdAsync(int orderId)
        {
            var invoice = await _unitOfWork.Invoices.GetByOrderIdAsync(orderId);
            return invoice != null ? _mapper.Map<InvoiceDto>(invoice) : null;
        }
    }
}

