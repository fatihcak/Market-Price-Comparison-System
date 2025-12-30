using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Services;
using DTOs.DTOs.Responses;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;
using FluentAssertions;

using Microsoft.Extensions.Logging;

namespace BACKEND.Tests.UnitTests.Services;

public class ChatServiceTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<IMarketRepository> _mockMarketRepository;
    private readonly Mock<IPriceRepository> _mockPriceRepository;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly Mock<Microsoft.Extensions.Caching.Memory.IMemoryCache> _mockMemoryCache;
    private readonly Mock<ILogger<ChatService>> _mockLogger;
    private readonly ChatService _service;

    public ChatServiceTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockProductRepository = new Mock<IProductRepository>();
        _mockMarketRepository = new Mock<IMarketRepository>();
        _mockPriceRepository = new Mock<IPriceRepository>();
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _mockMemoryCache = new Mock<Microsoft.Extensions.Caching.Memory.IMemoryCache>();
        _mockLogger = new Mock<ILogger<ChatService>>();

        _mockConfiguration.Setup(c => c["AiSettings:GoogleApiKey"]).Returns("test_api_key");
        
        // Mock Cache methods to return null (cache miss) or valid objects as needed
        object outValue;
        _mockMemoryCache.Setup(m => m.TryGetValue(It.IsAny<object>(), out outValue)).Returns(false);
        var mockCacheEntry = new Mock<Microsoft.Extensions.Caching.Memory.ICacheEntry>();
        _mockMemoryCache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(mockCacheEntry.Object);

        var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        _service = new ChatService(
            _mockConfiguration.Object,
            httpClient,
            _mockProductRepository.Object,
            _mockMarketRepository.Object,
            _mockPriceRepository.Object,
            _mockMemoryCache.Object,
            _mockLogger.Object);
    }

    // this checks if the chat works correctly
    [Fact]
    public async Task when_intent_is_chat_return_simple_reply()
    {
        // Arrange
        var userMessage = "Hello";
        var sessionId = "test-session";
        
        // mocked response from python service
        var analysisResponse = new
        {
            candidates = new[]
            {
                new { content = new { parts = new[] { new { text = "{\"intent\": \"chat\", \"reply\": \"Hello there!\"}" } } } }
            }
        };

        var chatResponse = new
        {
            candidates = new[]
            {
                new { content = new { parts = new[] { new { text = "Hello there!" } } } }
            }
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(analysisResponse))
            });

        // Act
        var result = await _service.GetChatResponseAsync(userMessage, sessionId);

        // Assert
        // Assert.Equal("Hello there!", result.Reply);
        result.Reply.Should().Be("Hello there!"); // switching to fluent assertions
        Assert.Null(result.BasketSuggestion);
    }

    [Fact]
    public async Task GetChatResponseAsync_WhenIntentIsSmartBasket_ReturnsBasketSuggestion()
    {
        // Arrange
        var userMessage = "Cheap apple";
        var sessionId = "test-session";
        var items = new List<string> { "apple" };
        var analysisResponse = new
        {
            candidates = new[]
            {
                new { content = new { parts = new[] { new { text = "{\"intent\": \"smart_basket\", \"items\": [\"apple\"]}" } } } }
            }
        };

        // Mock Repositories
        var products = new List<Product>
        {
            new Product { Id = 1, ProductName = "Golden Apple" }
        };
        var markets = new List<Market>
        {
            new Market { Id = 1, MarketName = "MarketA" },
            new Market { Id = 2, MarketName = "MarketB" }
        };
        var prices = new List<MarketProductPrice>
        {
            new MarketProductPrice { ProductId = 1, MarketId = 1, Price = 10 },
            new MarketProductPrice { ProductId = 1, MarketId = 2, Price = 20 }
        };

        _mockProductRepository.Setup(r => r.SearchByNamesAsync(It.IsAny<List<string>>())).ReturnsAsync(products);
        _mockPriceRepository.Setup(r => r.GetPricesForProductsAsync(It.IsAny<IEnumerable<int>>())).ReturnsAsync(prices);
        _mockMarketRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(markets);

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(analysisResponse))
            });

        // Act
        var result = await _service.GetChatResponseAsync(userMessage, sessionId);

        // Assert
        Assert.NotNull(result.BasketSuggestion);
        Assert.Equal("MarketA", result.BasketSuggestion.CheapestMarketName);
        Assert.Equal(10, result.BasketSuggestion.TotalPrice);
    }
}
