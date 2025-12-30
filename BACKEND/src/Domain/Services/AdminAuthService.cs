using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace Domain.Services;

/// <summary>
/// Service for admin authentication with JWT token generation
/// </summary>
public class AdminAuthService
{
    private readonly IAdminUserRepository _repository;
    private readonly IConfiguration _configuration;
    private readonly LoginThrottlingService _throttlingService;

    public AdminAuthService(
        IAdminUserRepository repository, 
        IConfiguration configuration,
        LoginThrottlingService throttlingService)
    {
        _repository = repository;
        _configuration = configuration;
        _throttlingService = throttlingService;
    }

    /// <summary>
    /// Authenticates admin user and returns JWT token
    /// </summary>
    /// <param name="username">Admin username</param>
    /// <param name="password">Admin password</param>
    /// <returns>JWT token if successful, null if failed</returns>
    /// <exception cref="InvalidOperationException">Thrown when account is locked out</exception>
    public async Task<string?> LoginAsync(string username, string password)
    {
        // Check if user is locked out
        if (_throttlingService.IsLockedOut(username))
        {
            var remainingTime = _throttlingService.GetRemainingLockoutTime(username);
            throw new InvalidOperationException(
                $"Account is locked. Try again in {remainingTime?.Minutes ?? 0} minutes.");
        }

        var admin = await _repository.GetByUsernameAsync(username);
        if (admin == null || !BCrypt.Net.BCrypt.Verify(password, admin.PasswordHash))
        {
            // Record failed attempt
            _throttlingService.RecordFailedAttempt(username);
            
            var attempts = _throttlingService.GetFailedAttemptCount(username);
            if (attempts >= 3)
            {
                // Warn user about remaining attempts
                throw new InvalidOperationException(
                    $"Invalid credentials. {5 - attempts} attempts remaining before lockout.");
            }
            
            return null;
        }

        // Clear failed attempts on successful login
        _throttlingService.ClearFailedAttempts(username);

        admin.LastLogin = DateTime.UtcNow;
        await _repository.SaveChangesAsync();

        return GenerateJwtToken(admin);
    }

    /// <summary>
    /// Creates a new admin user
    /// </summary>
    /// <param name="username">Admin username</param>
    /// <param name="password">Admin password (must meet complexity requirements)</param>
    /// <returns>Created admin user</returns>
    public async Task<AdminUser> CreateAdminAsync(string username, string password)
    {
        // Validate password complexity
        ValidatePassword(password);
        
        if (await _repository.ExistsAsync(username))
        {
            throw new ArgumentException("Username already exists");
        }

        var admin = new AdminUser
        {
            Username = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(admin);
        await _repository.SaveChangesAsync();

        return admin;
    }

    private static void ValidatePassword(string password)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(password))
            errors.Add("Password cannot be empty");
        else
        {
            if (password.Length < 8)
                errors.Add("Password must be at least 8 characters");
            if (!password.Any(char.IsUpper))
                errors.Add("Password must contain at least one uppercase letter");
            if (!password.Any(char.IsLower))
                errors.Add("Password must contain at least one lowercase letter");
            if (!password.Any(char.IsDigit))
                errors.Add("Password must contain at least one digit");
            if (!password.Any(c => !char.IsLetterOrDigit(c)))
                errors.Add("Password must contain at least one special character");
        }

        if (errors.Any())
            throw new ArgumentException(string.Join("; ", errors));
    }

    private string GenerateJwtToken(AdminUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("role", "Admin")
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JwtSettings:ExpirationInMinutes"]!)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

