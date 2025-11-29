using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Services;
using DTOs.DTOs.Requests;
using Moq;
using Xunit;

namespace BACKEND.Tests.UnitTests.Services;

public class MarketServiceTests
{
    private readonly Mock<IMarketRepository> _mockMarketRepository;
    private readonly MarketService _marketService;

    public MarketServiceTests()
    {
        _mockMarketRepository = new Mock<IMarketRepository>();
        _marketService = new MarketService(_mockMarketRepository.Object);
    }

    [Fact]
    public async Task GetMarketById_ExistingId_ReturnsMarket()
    {
        // Arrange
        var marketId = 1;
        var expectedMarket = new Market
        {
            Id = marketId,
            MarketName = "Market 1",
            LogoUrl = "test.jpg",
            Website = "test.com"
        };

        _mockMarketRepository
            .Setup(repo => repo.GetByIdAsync(marketId))
            .ReturnsAsync(expectedMarket);

        // Act
        var result = await _marketService.GetMarketByIdAsync(marketId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedMarket.MarketName, result.MarketName);
        Assert.Equal(expectedMarket.LogoUrl, result.LogoUrl);
        Assert.Equal(expectedMarket.Website, result.Website);
        _mockMarketRepository.Verify(repo => repo.GetByIdAsync(marketId), Times.Once);
    }

    [Fact]
    public async Task GetMarketById_NonExistingId_ReturnsNull()
    {
        // Arrange
        var marketId = 999;
        _mockMarketRepository
            .Setup(repo => repo.GetByIdAsync(marketId))
            .ReturnsAsync((Market?)null);

        // Act
        var result = await _marketService.GetMarketByIdAsync(marketId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateMarket_ValidData_ReturnsCreatedMarket()
    {
        // Arrange
        var createDto = new CreateMarketDTO
        {
            MarketName = "New Market",
            LogoUrl = "new.jpg",
            Website = "new.com"
        };

        _mockMarketRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Market>()))
            .ReturnsAsync((Market m) => m);

        // Act
        var result = await _marketService.CreateMarketAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createDto.MarketName, result.MarketName);
        _mockMarketRepository.Verify(repo => repo.AddAsync(It.IsAny<Market>()), Times.Once);
    }

    [Fact]
    public async Task UpdateMarket_ValidData_UpdatesMarket()
    {
        // Arrange
        var marketId = 1;
        var updateDto = new UpdateMarketDTO
        {
            MarketName = "Updated Market",
            LogoUrl = "updated.jpg",
            Website = "updated.com"
        };

        var existingMarket = new Market { Id = marketId, MarketName = "Old Market" };

        _mockMarketRepository.Setup(repo => repo.GetByIdAsync(marketId)).ReturnsAsync(existingMarket);
        _mockMarketRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Market>())).Returns(Task.CompletedTask);

        // Act
        var result = await _marketService.UpdateMarketAsync(marketId, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updateDto.MarketName, result.MarketName);
        _mockMarketRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Market>()), Times.Once);
    }

    [Fact]
    public async Task DeleteMarket_ExistingId_PerformsSoftDelete()
    {
        // Arrange
        var marketId = 1;
        var existingMarket = new Market { Id = marketId, MarketName = "To Delete" };

        _mockMarketRepository.Setup(repo => repo.GetByIdAsync(marketId)).ReturnsAsync(existingMarket);
        _mockMarketRepository.Setup(repo => repo.DeleteAsync(marketId)).Returns(Task.CompletedTask);

        // Act
        var result = await _marketService.DeleteMarketAsync(marketId);

        // Assert
        Assert.True(result);
        _mockMarketRepository.Verify(repo => repo.DeleteAsync(marketId), Times.Once);
    }
}