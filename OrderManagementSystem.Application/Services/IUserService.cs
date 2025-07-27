using OrderManagementSystem.Application.DTOs;

namespace OrderManagementSystem.Application.Services
{
    public interface IUserService
    {
        Task<UserDto> RegisterUserAsync(RegisterUserDto registerUserDto);
        Task<AuthResponseDto?> LoginUserAsync(LoginUserDto loginUserDto);
        Task<UserDto?> GetUserByIdAsync(int userId);
        Task<bool> UsernameExistsAsync(string username);
    }
}

