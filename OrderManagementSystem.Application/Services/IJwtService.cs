using OrderManagementSystem.Domain.Entities;

namespace OrderManagementSystem.Application.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        bool ValidateToken(string token);
        int GetUserIdFromToken(string token);
    }
}

