namespace OrderManagementSystem.Domain.Services
{
    public class DiscountService : IDiscountService
    {
        public decimal CalculateDiscount(decimal orderTotal)
        {
            // Tiered discount logic: 5% off for orders over $100, 10% off for orders over $200
            if (orderTotal >= 200)
                return 10;
            else if (orderTotal >= 100)
                return 5;
            else
                return 0;
        }

        public decimal ApplyDiscount(decimal orderTotal, decimal discountPercentage)
        {
            if (discountPercentage < 0 || discountPercentage > 100)
                throw new ArgumentException("Discount percentage must be between 0 and 100", nameof(discountPercentage));

            return orderTotal * (1 - discountPercentage / 100);
        }
    }
}

