namespace OrderManagementSystem.Domain.Services
{
    public interface IDiscountService
    {
        decimal CalculateDiscount(decimal orderTotal);
        decimal ApplyDiscount(decimal orderTotal, decimal discountPercentage);
    }
}

