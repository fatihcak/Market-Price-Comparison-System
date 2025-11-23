using DTOs.DTOs.Requests;
using DTOs.DTOs.Responses;

namespace Domain.Interfaces.Services;

public interface ICategoryService
{
    Task<IEnumerable<CategoryResponseDTO>> GetAllCategoriesAsync();
    Task<CategoryResponseDTO?> GetCategoryByIdAsync(int id);
    Task<CategoryResponseDTO> CreateCategoryAsync(CreateCategoryDTO dto);
    Task<CategoryResponseDTO?> UpdateCategoryAsync(int id, UpdateCategoryDTO dto);
    Task<bool> DeleteCategoryAsync(int id);
}
