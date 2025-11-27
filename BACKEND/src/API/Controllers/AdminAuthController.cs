using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

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
        var token = await _authService.LoginAsync(request.Username, request.Password);
        if (token == null)
        {
            return Unauthorized("Invalid username or password");
        }

        return Ok(new { Token = token });
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateAdmin([FromBody] LoginRequest request)
    {
        // This endpoint should be protected or removed in production!
        // For now, we allow creating the first admin.
        try
        {
            var admin = await _authService.CreateAdminAsync(request.Username, request.Password);
            return Ok(new { admin.Username, admin.CreatedAt });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
