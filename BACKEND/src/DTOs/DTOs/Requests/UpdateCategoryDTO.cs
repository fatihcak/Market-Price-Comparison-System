using System.ComponentModel.DataAnnotations;

namespace DTOs.DTOs.Requests;

public class UpdateCategoryDTO
{
    [Required(ErrorMessage = "Category name is required")]
    [MaxLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
    public string CategoryName { get; set; } = string.Empty;
}
