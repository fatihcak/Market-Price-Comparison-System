using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace Domain.Services;

public class AdminAuthService
{
    private readonly IAdminUserRepository _repository;
    private readonly IConfiguration _configuration;

    public AdminAuthService(IAdminUserRepository repository, IConfiguration configuration)
    {
        _repository = repository;
        _configuration = configuration;
    }

    public async Task<string?> LoginAsync(string username, string password)
    {
        var admin = await _repository.GetByUsernameAsync(username);
        if (admin == null || !BCrypt.Net.BCrypt.Verify(password, admin.PasswordHash))
        {
            return null;
        }

        admin.LastLogin = DateTime.UtcNow;
        await _repository.SaveChangesAsync();

        return GenerateJwtToken(admin);
    }

    public async Task<AdminUser> CreateAdminAsync(string username, string password)
    {
        if (await _repository.ExistsAsync(username))
        {
            throw new Exception("Username already exists");
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
