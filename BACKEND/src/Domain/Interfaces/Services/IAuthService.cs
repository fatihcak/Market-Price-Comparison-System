using DTOs.DTOs.Auth;

namespace Domain.Interfaces.Services;

public interface IAuthService
{
    Task<string?> LoginAsync(LoginRequestDTO loginRequest);
    Task<bool> RegisterAsync(RegisterRequestDTO registerRequest);
    string GenerateJwtToken(int userId, string email, string role);
}