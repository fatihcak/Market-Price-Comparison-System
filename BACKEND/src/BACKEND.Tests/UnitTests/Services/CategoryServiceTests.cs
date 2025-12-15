using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Services;
using DTOs.DTOs.Requests;
using DTOs.DTOs.Responses;
using Moq;
using Xunit;

namespace BACKEND.Tests.UnitTests.Services;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _mockRepository;
    private readonly CategoryService _service;

    public CategoryServiceTests()
    {
        _mockRepository = new Mock<ICategoryRepository>();
        _service = new CategoryService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAllCategoriesAsync_ReturnsMappedCategories()
    {
        // Arrange
        var categories = new List<ProductCategory>
        {
            new ProductCategory { Id = 1, CategoryName = "Cat1" },
            new ProductCategory { Id = 2, CategoryName = "Cat2" }
        };

        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

        // Act
        var result = await _service.GetAllCategoriesAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, c => c.CategoryName == "Cat1");
    }

    [Fact]
    public async Task GetCategoryByIdAsync_WhenExists_ReturnsCategory()
    {
        // Arrange
        var category = new ProductCategory { Id = 1, CategoryName = "Cat1" };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(category);

        // Act
        var result = await _service.GetCategoryByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetCategoryByIdAsync_WhenNotExists_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((ProductCategory?)null);

        // Act
        var result = await _service.GetCategoryByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateCategoryAsync_CreatesAndReturnsCategory()
    {
        // Arrange
        var dto = new CreateCategoryDTO { CategoryName = "NewCat" };
        
        // Act
        var result = await _service.CreateCategoryAsync(dto);

        // Assert
        Assert.Equal("NewCat", result.CategoryName);
        _mockRepository.Verify(r => r.AddAsync(It.Is<ProductCategory>(c => c.CategoryName == "NewCat")), Times.Once);
    }

    [Fact]
    public async Task UpdateCategoryAsync_WhenExists_UpdatesAndReturnsCategory()
    {
        // Arrange
        var category = new ProductCategory { Id = 1, CategoryName = "OldName" };
        var dto = new UpdateCategoryDTO { CategoryName = "NewName" };

        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(category);

        // Act
        var result = await _service.UpdateCategoryAsync(1, dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("NewName", result.CategoryName);
        _mockRepository.Verify(r => r.UpdateAsync(category), Times.Once);
    }

    [Fact]
    public async Task UpdateCategoryAsync_WhenNotExists_ReturnsNull()
    {
        // Arrange
        var dto = new UpdateCategoryDTO { CategoryName = "NewName" };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((ProductCategory?)null);

        // Act
        var result = await _service.UpdateCategoryAsync(1, dto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteCategoryAsync_WhenExists_ReturnsTrue()
    {
        // Arrange
        var category = new ProductCategory { Id = 1 };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(category);

        // Act
        var result = await _service.DeleteCategoryAsync(1);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteCategoryAsync_WhenNotExists_ReturnsFalse()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((ProductCategory?)null);

        // Act
        var result = await _service.DeleteCategoryAsync(1);

        // Assert
        Assert.False(result);
    }
}
