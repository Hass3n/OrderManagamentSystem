using OrderManagementSystem.Domain.Services;
using Xunit;

namespace OrderManagementSystem.Tests.UnitTests
{
    public class DiscountServiceTests
    {
        private readonly DiscountService _discountService;

        public DiscountServiceTests()
        {
            _discountService = new DiscountService();
        }

        [Theory]
        [InlineData(50, 0)]
        [InlineData(99.99, 0)]
        [InlineData(100, 5)]
        [InlineData(150, 5)]
        [InlineData(199.99, 5)]
        [InlineData(200, 10)]
        [InlineData(300, 10)]
        public void CalculateDiscount_ShouldReturnCorrectDiscountPercentage(decimal orderTotal, decimal expectedDiscount)
        {
            // Act
            var result = _discountService.CalculateDiscount(orderTotal);

            // Assert
            Assert.Equal(expectedDiscount, result);
        }

        [Theory]
        [InlineData(100, 10, 90)]
        [InlineData(200, 5, 190)]
        [InlineData(150, 0, 150)]
        public void ApplyDiscount_ShouldReturnCorrectDiscountedAmount(decimal orderTotal, decimal discountPercentage, decimal expectedAmount)
        {
            // Act
            var result = _discountService.ApplyDiscount(orderTotal, discountPercentage);

            // Assert
            Assert.Equal(expectedAmount, result);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(101)]
        public void ApplyDiscount_ShouldThrowException_WhenDiscountPercentageIsInvalid(decimal invalidDiscountPercentage)
        {
            // Act & Assert
            Assert.Throws<System.ArgumentException>(() => _discountService.ApplyDiscount(100, invalidDiscountPercentage));
        }
    }
}

