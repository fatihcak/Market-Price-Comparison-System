using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using DTOs.DTOs.Requests;
using DTOs.DTOs.Responses;

namespace Domain.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<CategoryResponseDTO>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return categories.Select(MapToResponseDTO);
    }

    public async Task<CategoryResponseDTO?> GetCategoryByIdAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        return category != null ? MapToResponseDTO(category) : null;
    }

    public async Task<CategoryResponseDTO> CreateCategoryAsync(CreateCategoryDTO dto)
    {
        var category = new ProductCategory
        {
            CategoryName = dto.CategoryName,
            CreatedAt = DateTime.UtcNow
        };

        await _categoryRepository.AddAsync(category);
        return MapToResponseDTO(category);
    }

    public async Task<CategoryResponseDTO?> UpdateCategoryAsync(int id, UpdateCategoryDTO dto)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null) return null;

        category.CategoryName = dto.CategoryName;
        await _categoryRepository.UpdateAsync(category);

        return MapToResponseDTO(category);
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null) return false;

        await _categoryRepository.DeleteAsync(id);
        return true;
    }

    private static CategoryResponseDTO MapToResponseDTO(ProductCategory category)
    {
        return new CategoryResponseDTO
        {
            Id = category.Id,
            CategoryName = category.CategoryName,
            CreatedAt = category.CreatedAt
        };
    }
}
