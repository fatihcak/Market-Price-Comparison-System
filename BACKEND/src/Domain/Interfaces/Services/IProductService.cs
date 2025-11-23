using DTOs.DTOs.Requests;
using DTOs.DTOs.Responses;

namespace Domain.Interfaces.Services;

public interface IProductService
{
    Task<IEnumerable<ProductResponseDTO>> GetAllProductsAsync();
    Task<ProductResponseDTO?> GetProductByIdAsync(int id);
    Task<IEnumerable<ProductResponseDTO>> GetProductsByCategoryAsync(int categoryId);
    Task<IEnumerable<ProductResponseDTO>> SearchProductsAsync(string searchTerm);
    Task<IEnumerable<ProductResponseDTO>> SearchByBrandAsync(string brand);
    Task<ProductResponseDTO> CreateProductAsync(CreateProductDTO dto);
    Task<ProductResponseDTO?> UpdateProductAsync(int id, UpdateProductDTO dto);
    Task<bool> DeleteProductAsync(int id);
}
