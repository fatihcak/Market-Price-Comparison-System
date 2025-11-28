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