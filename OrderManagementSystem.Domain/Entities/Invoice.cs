using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManagementSystem.Domain.Entities
{
    public class Invoice
    {
        [Key]
        public int InvoiceId { get; set; }
        
        [Required]
        public int OrderId { get; set; }
        
        [Required]
        public DateTime InvoiceDate { get; set; }
        
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Total amount cannot be negative")]
        public decimal TotalAmount { get; set; }
        
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; } = null!;
    }
}

