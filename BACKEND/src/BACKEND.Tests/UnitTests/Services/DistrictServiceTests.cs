using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Services;
using DTOs.DTOs.Responses;
using Moq;
using Xunit;

namespace BACKEND.Tests.UnitTests.Services;

public class DistrictServiceTests
{
    private readonly Mock<IDistrictRepository> _mockRepository;
    private readonly DistrictService _service;

    public DistrictServiceTests()
    {
        _mockRepository = new Mock<IDistrictRepository>();
        _service = new DistrictService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetDistrictsByCityIdAsync_ReturnsMappedDistricts()
    {
        // Arrange
        var cityId = 1;
        var districts = new List<District>
        {
            new District { Id = 1, CityId = cityId, DistrictName = "Kadikoy" },
            new District { Id = 2, CityId = cityId, DistrictName = "Besiktas" }
        };

        _mockRepository.Setup(r => r.GetByCityIdAsync(cityId)).ReturnsAsync(districts);

        // Act
        var result = await _service.GetDistrictsByCityIdAsync(cityId);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, d => d.DistrictName == "Kadikoy");
    }

    [Fact]
    public async Task GetDistrictByIdAsync_WhenExists_ReturnsDistrict()
    {
        // Arrange
        var district = new District { Id = 1, CityId = 1, DistrictName = "Kadikoy" };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(district);

        // Act
        var result = await _service.GetDistrictByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Kadikoy", result.DistrictName);
    }

    [Fact]
    public async Task GetDistrictByIdAsync_WhenNotExists_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((District?)null);

        // Act
        var result = await _service.GetDistrictByIdAsync(1);

        // Assert
        Assert.Null(result);
    }
}
