using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Services;
using DTOs.DTOs.Requests;
using DTOs.DTOs.Responses;
using Moq;
using Xunit;

namespace BACKEND.Tests.UnitTests.Services;

public class ShoppingListServiceTests
{
    private readonly Mock<IShoppingListRepository> _mockRepository;
    private readonly ShoppingListService _service;

    public ShoppingListServiceTests()
    {
        _mockRepository = new Mock<IShoppingListRepository>();
        _service = new ShoppingListService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetShoppingListBySessionIdAsync_ReturnsItems()
    {
        // Arrange
        var items = new List<UserProductList>
        {
            new UserProductList { Id = 1, SessionId = "s1" }
        };
        _mockRepository.Setup(r => r.GetBySessionIdAsync("s1")).ReturnsAsync(items);

        // Act
        var result = await _service.GetShoppingListBySessionIdAsync("s1");

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task AddItemToShoppingListAsync_ExistingItem_IncrementsQuantity()
    {
        // Arrange
        var request = new CreateShoppingListDTO { SessionId = "s1", ProductId = 1, Quantity = 2 };
        var existingItem = new UserProductList { Id = 1, SessionId = "s1", ProductId = 1, Quantity = 1 };

        _mockRepository.Setup(r => r.GetBySessionAndProductIdAsync("s1", 1)).ReturnsAsync(existingItem);
        _mockRepository.Setup(r => r.UpdateAsync(existingItem)).Returns(Task.CompletedTask);

        // Act
        var result = await _service.AddItemToShoppingListAsync(request);

        // Assert
        Assert.Equal(3, result.Quantity);
        _mockRepository.Verify(r => r.UpdateAsync(existingItem), Times.Once);
    }

    [Fact]
    public async Task AddItemToShoppingListAsync_NewItem_AddsItem()
    {
        // Arrange
        var request = new CreateShoppingListDTO { SessionId = "s1", ProductId = 1, Quantity = 2 };

        _mockRepository.Setup(r => r.GetBySessionAndProductIdAsync("s1", 1)).ReturnsAsync((UserProductList?)null);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<UserProductList>()))
            .ReturnsAsync((UserProductList item) => item);

        // Act
        var result = await _service.AddItemToShoppingListAsync(request);

        // Assert
        Assert.Equal(2, result.Quantity);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<UserProductList>()), Times.Once);
    }

    [Fact]
    public async Task RemoveItemFromShoppingListAsync_WhenExists_ReturnsTrue()
    {
        // Arrange
        var item = new UserProductList { Id = 1 };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);

        // Act
        var result = await _service.RemoveItemFromShoppingListAsync(1);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task ClearShoppingListAsync_ClearsItems()
    {
        // Arrange
        var items = new List<UserProductList>
        {
            new UserProductList { Id = 1 },
            new UserProductList { Id = 2 }
        };
        _mockRepository.Setup(r => r.GetBySessionIdAsync("s1")).ReturnsAsync(items);

        // Act
        var result = await _service.ClearShoppingListAsync("s1");

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Exactly(2));
    }
}
