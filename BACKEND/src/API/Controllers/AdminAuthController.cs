using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using API.Extensions;

namespace API.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin")]
[Route("api/admin")]
[ApiController]
public class AdminAuthController : ControllerBase
{
    private readonly AdminAuthService _authService;

    public AdminAuthController(AdminAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var token = await _authService.LoginAsync(request.Username, request.Password);
            if (token == null)
            {
                return this.ApiUnauthorized("Invalid username or password");
            }

            return this.ApiOk(new { Token = token });
        }
        catch (InvalidOperationException ex)
        {
            // Account locked or too many failed attempts
            return this.ApiBadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Create a new admin user (requires existing admin authentication)
    /// </summary>
    [HttpPost("create")]
    //[Authorize]
    public async Task<IActionResult> CreateAdmin([FromBody] LoginRequest request)
    {
        try
        {
            var admin = await _authService.CreateAdminAsync(request.Username, request.Password);
            return this.ApiOk(new { admin.Username, admin.CreatedAt });
        }
        catch (ArgumentException ex)
        {
            return this.ApiBadRequest(ex.Message);
        }
        catch (Exception)
        {
            return this.ApiBadRequest("Failed to create admin");
        }
    }
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

