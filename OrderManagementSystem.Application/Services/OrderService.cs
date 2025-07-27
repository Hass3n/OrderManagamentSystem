using AutoMapper;
using OrderManagementSystem.Application.DTOs;
using OrderManagementSystem.Domain.Entities;
using OrderManagementSystem.Domain.Enums;
using OrderManagementSystem.Domain.Interfaces;
using OrderManagementSystem.Domain.Services;

namespace OrderManagementSystem.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDiscountService _discountService;
        private readonly IEmailService _emailService;
        private readonly IInvoiceService _invoiceService;

        public OrderService(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            IDiscountService discountService,
            IEmailService emailService,
            IInvoiceService invoiceService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _discountService = discountService;
            _emailService = emailService;
            _invoiceService = invoiceService;
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto)
        {
            // Validate customer exists
            var customer = await _unitOfWork.Customers.GetByIdAsync(createOrderDto.CustomerId);
            if (customer == null)
            {
                throw new ArgumentException($"Customer with ID {createOrderDto.CustomerId} not found");
            }

            // Validate products and stock availability
            var productIds = createOrderDto.OrderItems.Select(oi => oi.ProductId).ToList();
            var products = await _unitOfWork.Products.GetProductsByIdsAsync(productIds);
            
            if (products.Count() != productIds.Count)
            {
                throw new ArgumentException("One or more products not found");
            }

            // Check stock availability
            foreach (var orderItem in createOrderDto.OrderItems)
            {
                var product = products.First(p => p.ProductId == orderItem.ProductId);
                if (product.Stock < orderItem.Quantity)
                {
                    throw new InvalidOperationException($"Insufficient stock for product {product.Name}. Available: {product.Stock}, Requested: {orderItem.Quantity}");
                }
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Create order
                var order = _mapper.Map<Order>(createOrderDto);
                order.OrderDate = DateTime.UtcNow;
                order.Status = OrderStatus.Pending;

                // Create order items and calculate total
                decimal totalAmount = 0;
                foreach (var orderItemDto in createOrderDto.OrderItems)
                {
                    var product = products.First(p => p.ProductId == orderItemDto.ProductId);
                    var orderItem = new OrderItem
                    {
                        ProductId = orderItemDto.ProductId,
                        Quantity = orderItemDto.Quantity,
                        UnitPrice = product.Price,
                        Discount = 0 // Individual item discount can be applied here if needed
                    };

                    order.OrderItems.Add(orderItem);
                    totalAmount += orderItem.UnitPrice * orderItem.Quantity;

                    // Update product stock
                    await _unitOfWork.Products.UpdateStockAsync(product.ProductId, orderItem.Quantity);
                }

                // Apply tiered discount
                var discountPercentage = _discountService.CalculateDiscount(totalAmount);
                if (discountPercentage > 0)
                {
                    totalAmount = _discountService.ApplyDiscount(totalAmount, discountPercentage);
                }

                order.TotalAmount = totalAmount;

                await _unitOfWork.Orders.AddAsync(order);
                await _unitOfWork.SaveChangesAsync();

                // Generate invoice
                await _invoiceService.CreateInvoiceAsync(order.OrderId);

                // Send confirmation email
                await _emailService.SendOrderConfirmationAsync(customer.Email, order.OrderId);

                await _unitOfWork.CommitTransactionAsync();

                return _mapper.Map<OrderDto>(order);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            return order != null ? _mapper.Map<OrderDto>(order) : null;
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _unitOfWork.Orders.GetAllAsync();
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByCustomerIdAsync(int customerId)
        {
            var orders = await _unitOfWork.Orders.GetOrdersByCustomerIdAsync(customerId);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto> UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new ArgumentException($"Order with ID {orderId} not found");
            }

            var oldStatus = order.Status;
            await _unitOfWork.Orders.UpdateOrderStatusAsync(orderId, status);
            await _unitOfWork.SaveChangesAsync();

            // Send status update email if status changed
            if (oldStatus != status)
            {
                await _emailService.SendOrderStatusUpdateAsync(order.Customer.Email, orderId, status.ToString());
            }

            order.Status = status;
            return _mapper.Map<OrderDto>(order);
        }

        public async Task<bool> OrderExistsAsync(int orderId)
        {
            return await _unitOfWork.Orders.ExistsAsync(orderId);
        }
    }
}

