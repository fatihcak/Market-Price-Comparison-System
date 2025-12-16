using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Services;
using DTOs.DTOs.Requests;
using DTOs.DTOs.Responses;
using Moq;
using Xunit;

namespace BACKEND.Tests.UnitTests.Services;

public class PriceServiceTests
{
    private readonly Mock<IPriceRepository> _mockRepository;
    private readonly PriceService _service;

    public PriceServiceTests()
    {
        _mockRepository = new Mock<IPriceRepository>();
        _service = new PriceService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetPricesByProductIdAsync_ReturnsPrices()
    {
        // Arrange
        var prices = new List<MarketProductPrice>
        {
            new MarketProductPrice { Id = 1, ProductId = 1, Price = 100 }
        };
        _mockRepository.Setup(r => r.GetByProductIdAsync(1)).ReturnsAsync(prices);

        // Act
        var result = await _service.GetPricesByProductIdAsync(1);

        // Assert
        Assert.Single(result);
        Assert.Equal(100, result.First().Price);
    }

    [Fact]
    public async Task AddPriceAsync_AddsAndReturnsPrice()
    {
        // Arrange
        var dto = new CreatePriceDTO { ProductId = 1, MarketId = 1, DistrictId = 1, Price = 50 };
        
        // Act
        var result = await _service.AddPriceAsync(dto);

        // Assert
        Assert.Equal(50, result.Price);
        _mockRepository.Verify(r => r.AddAsync(It.Is<MarketProductPrice>(p => p.Price == 50)), Times.Once);
    }

    [Fact]
    public async Task UpdatePriceAsync_WhenExists_UpdatesPrice()
    {
        // Arrange
        var existingPrice = new MarketProductPrice { Id = 1, Price = 50 };
        var dto = new UpdatePriceDTO { Price = 60 };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingPrice);

        // Act
        var result = await _service.UpdatePriceAsync(1, dto);

        // Assert
        Assert.Equal(60, result.Price);
        _mockRepository.Verify(r => r.UpdateAsync(existingPrice), Times.Once);
    }

    [Fact]
    public async Task UpdatePriceAsync_WhenNotExists_ReturnsNull()
    {
        // Arrange
        var dto = new UpdatePriceDTO { Price = 60 };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((MarketProductPrice?)null);

        // Act
        var result = await _service.UpdatePriceAsync(1, dto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeletePriceAsync_WhenExists_ReturnsTrue()
    {
        // Arrange
        var existingPrice = new MarketProductPrice { Id = 1 };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingPrice);

        // Act
        var result = await _service.DeletePriceAsync(1);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeletePriceAsync_WhenNotExists_ReturnsFalse()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((MarketProductPrice?)null);

        // Act
        var result = await _service.DeletePriceAsync(1);

        // Assert
        Assert.False(result);
    }
}
