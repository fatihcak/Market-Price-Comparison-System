using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Services;
using DTOs.DTOs.Responses;
using Moq;
using Xunit;

namespace BACKEND.Tests.UnitTests.Services;

public class BasketServiceTests
{
    private readonly Mock<IBasketRepository> _mockBasketRepository;
    private readonly Mock<IPriceRepository> _mockPriceRepository;
    private readonly Mock<IRepository<Market>> _mockMarketRepository;
    private readonly BasketService _service;

    public BasketServiceTests()
    {
        _mockBasketRepository = new Mock<IBasketRepository>();
        _mockPriceRepository = new Mock<IPriceRepository>();
        _mockMarketRepository = new Mock<IRepository<Market>>();

        _service = new BasketService(
            _mockBasketRepository.Object,
            _mockPriceRepository.Object,
            _mockMarketRepository.Object);
    }

    [Fact]
    public async Task AddToBasketAsync_NewItem_AddsToRepository()
    {
        // Arrange
        var sessionId = "session1";
        var productId = 1;
        var quantity = 2;

        _mockBasketRepository.Setup(r => r.GetBySessionAndProductIdAsync(sessionId, productId))
            .ReturnsAsync((UserProductList?)null);

        // Act
        await _service.AddToBasketAsync(sessionId, productId, quantity);

        // Assert
        _mockBasketRepository.Verify(r => r.AddAsync(It.Is<UserProductList>(i => 
            i.SessionId == sessionId && 
            i.ProductId == productId && 
            i.Quantity == quantity)), Times.Once);
    }

    [Fact]
    public async Task AddToBasketAsync_ExistingItem_UpdatesQuantity()
    {
        // Arrange
        var sessionId = "session1";
        var productId = 1;
        var quantity = 2;
        var existingItem = new UserProductList { Id = 1, SessionId = sessionId, ProductId = productId, Quantity = 1 };

        _mockBasketRepository.Setup(r => r.GetBySessionAndProductIdAsync(sessionId, productId))
            .ReturnsAsync(existingItem);

        // Act
        await _service.AddToBasketAsync(sessionId, productId, quantity);

        // Assert
        Assert.Equal(3, existingItem.Quantity);
        _mockBasketRepository.Verify(r => r.UpdateAsync(existingItem), Times.Once);
    }

    [Fact]
    public async Task AddToBasketAsync_QuantityZeroOrLess_RemovesItem()
    {
        // Arrange
        var sessionId = "session1";
        var productId = 1;
        var quantity = -1;
        var existingItem = new UserProductList { Id = 1, SessionId = sessionId, ProductId = productId, Quantity = 1 };

        _mockBasketRepository.Setup(r => r.GetBySessionAndProductIdAsync(sessionId, productId))
            .ReturnsAsync(existingItem);

        // Act
        await _service.AddToBasketAsync(sessionId, productId, quantity);

        // Assert
        _mockBasketRepository.Verify(r => r.DeleteAsync(existingItem.Id), Times.Once);
    }

    [Fact]
    public async Task GetBasketAsync_ReturnsBasketWithEstimatedPrices()
    {
        // Arrange
        var sessionId = "session1";
        var basketItems = new List<UserProductList>
        {
            new UserProductList 
            { 
                ProductId = 1, 
                Quantity = 2,
                Product = new Product { ProductName = "Product1" } 
            }
        };

        var prices = new List<MarketProductPrice>
        {
            new MarketProductPrice { ProductId = 1, Price = 100 },
            new MarketProductPrice { ProductId = 1, Price = 150 }
        };

        _mockBasketRepository.Setup(r => r.GetBySessionIdAsync(sessionId)).ReturnsAsync(basketItems);
        _mockPriceRepository.Setup(r => r.GetPricesForProductsAsync(It.IsAny<IEnumerable<int>>())).ReturnsAsync(prices);

        // Act
        var result = await _service.GetBasketAsync(sessionId);

        // Assert
        var item = result.First();
        Assert.Equal(125, item.EstimatedPrice); // (100+150)/2
        Assert.Equal(2, item.Quantity);
    }

    [Fact]
    public async Task CompareBasketAsync_CalculatesCheapestMarket()
    {
        // Arrange
        var sessionId = "session1";
        var basketItems = new List<UserProductList>
        {
            new UserProductList 
            { 
                ProductId = 1, 
                Quantity = 1,
                Product = new Product { ProductName = "Product1" } 
            }
        };

        var markets = new List<Market>
        {
            new Market { Id = 1, MarketName = "MarketCheaper" },
            new Market { Id = 2, MarketName = "MarketExpensive" }
        };

        var prices = new List<MarketProductPrice>
        {
            new MarketProductPrice { ProductId = 1, MarketId = 1, Price = 100 },
            new MarketProductPrice { ProductId = 1, MarketId = 2, Price = 200 }
        };

        _mockBasketRepository.Setup(r => r.GetBySessionIdAsync(sessionId)).ReturnsAsync(basketItems);
        _mockMarketRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(markets);
        _mockPriceRepository.Setup(r => r.GetPricesForProductsAsync(It.IsAny<IEnumerable<int>>())).ReturnsAsync(prices);

        // Act
        var result = await _service.CompareBasketAsync(sessionId);

        // Assert
        var cheapest = result.First(r => r.IsCheapest);
        Assert.Equal("MarketCheaper", cheapest.MarketName);
        Assert.Equal(100, cheapest.TotalPrice);
        
        var expensive = result.First(r => !r.IsCheapest);
        Assert.Equal("MarketExpensive", expensive.MarketName);
        Assert.Equal(200, expensive.TotalPrice);
    }
}
