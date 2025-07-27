using System.ComponentModel.DataAnnotations;
using OrderManagementSystem.Domain.Enums;

namespace OrderManagementSystem.Domain.Entities
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;
        
        [Required]
        public UserRole Role { get; set; }
    }
}

