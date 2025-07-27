using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.Application.DTOs;
using OrderManagementSystem.Application.Services;

namespace OrderManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoicesController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

       
        [HttpGet("{invoiceId}")]
        public async Task<ActionResult<InvoiceDto>> GetInvoice(int invoiceId)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(invoiceId);
            if (invoice == null)
            {
                return NotFound($"Invoice with ID {invoiceId} not found");
            }

            return Ok(invoice);
        }

      
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetAllInvoices()
        {
            var invoices = await _invoiceService.GetAllInvoicesAsync();
            return Ok(invoices);
        }

        [HttpGet("order/{orderId}")]
        public async Task<ActionResult<InvoiceDto>> GetInvoiceByOrderId(int orderId)
        {
            var invoice = await _invoiceService.GetInvoiceByOrderIdAsync(orderId);
            if (invoice == null)
            {
                return NotFound($"Invoice for order {orderId} not found");
            }

            return Ok(invoice);
        }
    }
}

