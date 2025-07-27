using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Domain.Entities;
using OrderManagementSystem.Domain.Interfaces;
using OrderManagementSystem.Infrastructure.Data;

namespace OrderManagementSystem.Infrastructure.Repositories
{
    public class InvoiceRepository : Repository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(OrderManagementDbContext context) : base(context)
        {
        }

        public async Task<Invoice?> GetByOrderIdAsync(int orderId)
        {
            return await _dbSet
                .Include(i => i.Order)
                    .ThenInclude(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                .Include(i => i.Order.Customer)
                .FirstOrDefaultAsync(i => i.OrderId == orderId);
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(i => i.InvoiceDate >= startDate && i.InvoiceDate <= endDate)
                .Include(i => i.Order)
                    .ThenInclude(o => o.Customer)
                .ToListAsync();
        }

        public override async Task<Invoice?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(i => i.Order)
                    .ThenInclude(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                .Include(i => i.Order.Customer)
                .FirstOrDefaultAsync(i => i.InvoiceId == id);
        }

        public override async Task<IEnumerable<Invoice>> GetAllAsync()
        {
            return await _dbSet
                .Include(i => i.Order)
                    .ThenInclude(o => o.Customer)
                .ToListAsync();
        }
    }
}

