using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace BACKEND.Tests.UnitTests.Services;

public class AdminAuthServiceTests
{
    private readonly Mock<IAdminUserRepository> _mockRepository;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly AdminAuthService _service;

    public AdminAuthServiceTests()
    {
        _mockRepository = new Mock<IAdminUserRepository>();
        _mockConfiguration = new Mock<IConfiguration>();

        // Setup configuration for JWT
        _mockConfiguration.Setup(c => c["JwtSettings:SecretKey"]).Returns("very_long_secret_key_for_testing_purposes_only");
        _mockConfiguration.Setup(c => c["JwtSettings:Issuer"]).Returns("TestIssuer");
        _mockConfiguration.Setup(c => c["JwtSettings:Audience"]).Returns("TestAudience");
        _mockConfiguration.Setup(c => c["JwtSettings:ExpirationInMinutes"]).Returns("60");

        _service = new AdminAuthService(_mockRepository.Object, _mockConfiguration.Object);
    }

    [Fact]
    public async Task LoginTest_HappyPath()
    {
        // Arrange
        var username = "admin";
        var password = "StrongP@ssw0rd!";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        var adminUser = new AdminUser { Username = username, PasswordHash = hashedPassword };

        _mockRepository.Setup(r => r.GetByUsernameAsync(username)).ReturnsAsync(adminUser);

        // Act
        var result = await _service.LoginAsync(username, password);

        // Assert
        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result));
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ReturnsNull()
    {
        // OLD CODE: var password = "password"; 
        // changed on 2024-12-28 because of new security policy
        var username = "admin";
        var password = "StrongP@ssw0rd!";
        var wrongPassword = "wrongpassword";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        var adminUser = new AdminUser { Username = username, PasswordHash = hashedPassword };

        _mockRepository.Setup(r => r.GetByUsernameAsync(username)).ReturnsAsync(adminUser);

        // Act
        var result = await _service.LoginAsync(username, wrongPassword);

        // Assert
        Assert.Null(result);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WithNonExistentUser_ReturnsNull()
    {
        // Arrange
        var username = "nonexistent";
        var password = "StrongP@ssw0rd!";

        _mockRepository.Setup(r => r.GetByUsernameAsync(username)).ReturnsAsync((AdminUser?)null);

        // Act
        var result = await _service.LoginAsync(username, password);

        // Assert
        Assert.Null(result);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    // This test was failing locally but fixed itself??
    [Fact]
    public async Task CreateAdminAsync_WhenUserDoesNotExist_CreatesUser()
    {
        // Arrange
        var username = "newadmin";
        var password = "StrongP@ssw0rd!";

        _mockRepository.Setup(r => r.ExistsAsync(username)).ReturnsAsync(false);

        // Act
        var result = await _service.CreateAdminAsync(username, password);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(username, result.Username);
        _mockRepository.Verify(r => r.AddAsync(It.Is<AdminUser>(u => u.Username == username)), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task admin_creation_fails_if_exists()
    {
        // Arrange
        var username = "existingadmin";
        var password = "StrongP@ssw0rd!";

        _mockRepository.Setup(r => r.ExistsAsync(username)).ReturnsAsync(true);

        // Act & Assert
        // TODO: Check if this exception message needs to be localized
        await Assert.ThrowsAsync<Exception>(() => _service.CreateAdminAsync(username, password));
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<AdminUser>()), Times.Never);
    }
}
