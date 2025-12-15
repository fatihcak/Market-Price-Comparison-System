using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Services;
using DTOs.DTOs.Requests;
using FluentAssertions;
using Moq;
using Xunit;

namespace BACKEND.Tests.UnitTests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _productService = new ProductService(_mockProductRepository.Object);
    }

    [Fact]
    public async Task CreateProductAsync_ShouldReturnProduct_WhenValidDto()
    {
        // Arrange
        var createDto = new CreateProductDTO
        {
            ProductName = "Test Milk",
            Brand = "Test Brand",
            CategoryId = 1,
            Unit = "1L"
        };

        var createdProduct = new Product
        {
            Id = 1,
            ProductName = createDto.ProductName,
            Brand = createDto.Brand,
            CategoryId = createDto.CategoryId,
            Unit = createDto.Unit,
            CreatedAt = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow,
            Category = new ProductCategory { Id = 1, CategoryName = "Dairy" }
        };

        _mockProductRepository.Setup(repo => repo.AddAsync(It.IsAny<Product>()))
            .Callback<Product>(p => p.Id = 1)
            .Returns<Product>(p => Task.FromResult(p));

        _mockProductRepository.Setup(repo => repo.GetProductWithCategoryAsync(1))
            .ReturnsAsync(createdProduct);

        // Act
        var result = await _productService.CreateProductAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.ProductName.Should().Be(createDto.ProductName);
        result.CategoryName.Should().Be("Dairy");
        
        _mockProductRepository.Verify(repo => repo.AddAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task GetAllProductsAsync_ReturnsMappedProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Id = 1, ProductName = "Product1", Category = new ProductCategory { CategoryName = "Cat1" } },
            new Product { Id = 2, ProductName = "Product2", Category = new ProductCategory { CategoryName = "Cat2" } }
        };

        _mockProductRepository.Setup(r => r.GetAllWithDetailsAsync()).ReturnsAsync(products);

        // Act
        var result = await _productService.GetAllProductsAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, p => p.ProductName == "Product1");
    }

    [Fact]
    public async Task GetProductByIdAsync_WhenExists_ReturnsProduct()
    {
        // Arrange
        var product = new Product { Id = 1, ProductName = "Product1", Category = new ProductCategory { CategoryName = "Cat1" } };
        _mockProductRepository.Setup(r => r.GetProductWithCategoryAsync(1)).ReturnsAsync(product);

        // Act
        var result = await _productService.GetProductByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Product1", result.ProductName);
    }

    [Fact]
    public async Task UpdateProductAsync_WhenExists_UpdatesAndReturnsProduct()
    {
        // Arrange
        var existingProduct = new Product { Id = 1, ProductName = "OldName", CategoryId = 1 };
        var dto = new UpdateProductDTO { ProductName = "NewName", Brand = "Brand", Unit = "Unit" };
        
        _mockProductRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingProduct);
        _mockProductRepository.Setup(r => r.GetProductWithCategoryAsync(1)).ReturnsAsync(existingProduct);

        // Act
        var result = await _productService.UpdateProductAsync(1, dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("NewName", existingProduct.ProductName);
        _mockProductRepository.Verify(r => r.UpdateAsync(existingProduct), Times.Once);
    }

    [Fact]
    public async Task UpdateProductAsync_WhenNotExists_ReturnsNull()
    {
        // Arrange
        var dto = new UpdateProductDTO { ProductName = "NewName" };
        _mockProductRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Product?)null);

        // Act
        var result = await _productService.UpdateProductAsync(1, dto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteProductAsync_WhenExists_ReturnsTrue()
    {
        // Arrange
        var product = new Product { Id = 1 };
        _mockProductRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

        // Act
        var result = await _productService.DeleteProductAsync(1);

        // Assert
        Assert.True(result);
        _mockProductRepository.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteProductAsync_WhenNotExists_ReturnsFalse()
    {
        // Arrange
        _mockProductRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Product?)null);

        // Act
        var result = await _productService.DeleteProductAsync(1);

        // Assert
        Assert.False(result);
    }
}
