using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using DTOs.DTOs.Requests;
using DTOs.DTOs.Responses;

namespace Domain.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<ProductResponseDTO>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Select(MapToResponseDTO);
    }

    public async Task<ProductResponseDTO?> GetProductByIdAsync(int id)
    {
        var product = await _productRepository.GetProductWithCategoryAsync(id);
        return product != null ? MapToResponseDTO(product) : null;
    }

    public async Task<IEnumerable<ProductResponseDTO>> GetProductsByCategoryAsync(int categoryId)
    {
        var products = await _productRepository.GetByCategoryIdAsync(categoryId);
        return products.Select(MapToResponseDTO);
    }

    public async Task<IEnumerable<ProductResponseDTO>> SearchProductsAsync(string searchTerm)
    {
        var products = await _productRepository.SearchByNameAsync(searchTerm);
        return products.Select(MapToResponseDTO);
    }

    public async Task<IEnumerable<ProductResponseDTO>> SearchByBrandAsync(string brand)
    {
        var products = await _productRepository.SearchByBrandAsync(brand);
        return products.Select(MapToResponseDTO);
    }

    public async Task<ProductResponseDTO> CreateProductAsync(CreateProductDTO dto)
    {
        var product = new Product
        {
            CategoryId = dto.CategoryId,
            ProductName = dto.ProductName,
            Brand = dto.Brand,
            Unit = dto.Unit,
            LastUpdated = DateTime.UtcNow
        };

        await _productRepository.AddAsync(product);

        // Get product with category for response
        var createdProduct = await _productRepository.GetProductWithCategoryAsync(product.Id);
        return MapToResponseDTO(createdProduct!);
    }

    public async Task<ProductResponseDTO?> UpdateProductAsync(int id, UpdateProductDTO dto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return null;
        }

        // Update only provided fields
        if (!string.IsNullOrWhiteSpace(dto.ProductName))
        {
            product.ProductName = dto.ProductName;
        }

        if (dto.Brand != null)
        {
            product.Brand = dto.Brand;
        }

        if (!string.IsNullOrWhiteSpace(dto.Unit))
        {
            product.Unit = dto.Unit;
        }

        product.LastUpdated = DateTime.UtcNow;

        await _productRepository.UpdateAsync(product);

        // Get product with category for response
        var updatedProduct = await _productRepository.GetProductWithCategoryAsync(id);
        return MapToResponseDTO(updatedProduct!);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return false;
        }

        await _productRepository.DeleteAsync(id); // Soft delete
        return true;
    }

    private static ProductResponseDTO MapToResponseDTO(Product product)
    {
        return new ProductResponseDTO
        {
            Id = product.Id,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.CategoryName ?? string.Empty,
            ProductName = product.ProductName,
            Brand = product.Brand,
            Unit = product.Unit,
            LastUpdated = product.LastUpdated,
            CreatedAt = product.CreatedAt
        };
    }
}
