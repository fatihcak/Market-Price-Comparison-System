using DTOs.DTOs.Users;

namespace Domain.Interfaces.Services;

public interface IUserService
{
    Task<UserDTO?> GetUserByIdAsync(int id);
    Task<IEnumerable<UserDTO>> GetAllUsersAsync();
}
