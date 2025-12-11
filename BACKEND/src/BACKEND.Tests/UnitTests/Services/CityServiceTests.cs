using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Services;
using DTOs.DTOs.Responses;
using Moq;
using Xunit;

namespace BACKEND.Tests.UnitTests.Services;

public class CityServiceTests
{
    private readonly Mock<ICityRepository> _mockRepository;
    private readonly CityService _service;

    public CityServiceTests()
    {
        _mockRepository = new Mock<ICityRepository>();
        _service = new CityService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAllCitiesAsync_ReturnsMappedCities()
    {
        // Arrange
        var cities = new List<City>
        {
            new City { Id = 1, CityName = "Istanbul" },
            new City { Id = 2, CityName = "Ankara" }
        };

        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(cities);

        // Act
        var result = await _service.GetAllCitiesAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, c => c.CityName == "Istanbul");
    }

    [Fact]
    public async Task GetCityByIdAsync_WhenExists_ReturnsCity()
    {
        // Arrange
        var city = new City { Id = 1, CityName = "Istanbul" };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(city);

        // Act
        var result = await _service.GetCityByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Istanbul", result.CityName);
    }

    [Fact]
    public async Task GetCityByIdAsync_WhenNotExists_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((City?)null);

        // Act
        var result = await _service.GetCityByIdAsync(1);

        // Assert
        Assert.Null(result);
    }
}
