using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OrderManagementSystem.Domain.Enums;

namespace OrderManagementSystem.Domain.Entities
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        
        [Required]
        public int CustomerId { get; set; }
        
        [Required]
        public DateTime OrderDate { get; set; }
        
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Total amount cannot be negative")]
        public decimal TotalAmount { get; set; }
        
        [Required]
        public PaymentMethod PaymentMethod { get; set; }
        
        [Required]
        public OrderStatus Status { get; set; }
        
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; } = null!;
        
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        
        public virtual Invoice? Invoice { get; set; }
    }
}

