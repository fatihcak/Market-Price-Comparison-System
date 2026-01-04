using DTOs.DTOs.Requests;
using DTOs.DTOs.Responses;

namespace Domain.Interfaces.Services;

public interface IProductService
{
    Task<IEnumerable<ProductResponseDTO>> GetAllProductsAsync();
    Task<(IEnumerable<ProductResponseDTO> Products, int TotalCount)> GetProductsWithPaginationAsync(int page, int pageSize);
    Task<ProductResponseDTO?> GetProductByIdAsync(int id);
    Task<IEnumerable<ProductResponseDTO>> GetProductsByCategoryAsync(int categoryId);
    Task<(IEnumerable<ProductResponseDTO> Products, int TotalCount)> GetProductsByCategoryWithPaginationAsync(int categoryId, int page, int pageSize);
    Task<IEnumerable<ProductResponseDTO>> SearchProductsAsync(string searchTerm);
    Task<(IEnumerable<ProductResponseDTO> Products, int TotalCount)> SearchProductsWithPaginationAsync(string searchTerm, int page, int pageSize);
    Task<IEnumerable<ProductResponseDTO>> SearchByBrandAsync(string brand);
    Task<(IEnumerable<ProductResponseDTO> Products, int TotalCount)> SearchByBrandWithPaginationAsync(string brand, int page, int pageSize);
    Task<ProductResponseDTO> CreateProductAsync(CreateProductDTO dto);
    Task<ProductResponseDTO?> UpdateProductAsync(int id, UpdateProductDTO dto);
    Task<bool> DeleteProductAsync(int id);
    Task<IEnumerable<ProductPriceHistoryDTO>> GetProductPriceHistoryAsync(int productId, int days);
    Task<(IEnumerable<ProductResponseDTO> Products, int TotalCount)> GetProductsOrderedByDiscountAsync(int page, int pageSize);
}
