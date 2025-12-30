using System.Net;
using System.Net.Http.Json;
using DataAccess.Data;
using Domain.Entities;
using DTOs.DTOs.Common;
using DTOs.DTOs.Responses;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BACKEND.Tests.IntegrationTests;

public class ProductIntegrationTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly ApiWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public ProductIntegrationTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllProducts_ReturnsSuccessAndList()
    {
        // Arrange
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            // Clear existing data (in case shared db)
            context.Product.RemoveRange(context.Product);
            context.ProductCategory.RemoveRange(context.ProductCategory);
            await context.SaveChangesAsync();

            // Seed test data
            var category = new ProductCategory { CategoryName = "TestCat" };
            context.ProductCategory.Add(category);
            await context.SaveChangesAsync();

            context.Product.Add(new Product { ProductName = "IntegrationTestProduct", CategoryId = category.Id, Brand = "Brand", Unit = "Unit" });
            await context.SaveChangesAsync();
        }

        // Act
        var response = await _client.GetAsync("/api/Product");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<ProductResponseDTO>>>();
        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data!.Should().Contain(p => p.ProductName == "IntegrationTestProduct");
    }

    [Fact]
    public async Task GetProductById_WhenExists_ReturnsSuccess()
    {
        // Arrange
        int productId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var category = new ProductCategory { CategoryName = "TestCat2" };
            context.ProductCategory.Add(category);
            await context.SaveChangesAsync();

            var product = new Product { ProductName = "SpecificProduct", CategoryId = category.Id };
            context.Product.Add(product);
            await context.SaveChangesAsync();
            productId = product.Id;
        }

        // Act
        var response = await _client.GetAsync($"/api/Product/{productId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<ProductResponseDTO>>();
        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data!.ProductName.Should().Be("SpecificProduct");
    }
}

