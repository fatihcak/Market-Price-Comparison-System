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

        // Setup repository to simulate adding the product
        _mockProductRepository.Setup(repo => repo.AddAsync(It.IsAny<Product>()))
            .Callback<Product>(p => p.Id = 1) // Simulate DB generating ID
            .Returns<Product>(p => Task.FromResult(p));

        // Setup repository to return the product with category after creation
        _mockProductRepository.Setup(repo => repo.GetProductWithCategoryAsync(1))
            .ReturnsAsync(createdProduct);

        // Act
        var result = await _productService.CreateProductAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.ProductName.Should().Be(createDto.ProductName);
        result.CategoryName.Should().Be("Dairy");
        
        // Verify that AddAsync was called once
        _mockProductRepository.Verify(repo => repo.AddAsync(It.IsAny<Product>()), Times.Once);
    }
}
