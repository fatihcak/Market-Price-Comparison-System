using System.ComponentModel.DataAnnotations;

namespace DTOs.DTOs.Requests;

public class CreateProductDTO
{
    [Required(ErrorMessage = "Category ID is required")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Product name is required")]
    [MaxLength(200, ErrorMessage = "Product name cannot exceed 200 characters")]
    public string ProductName { get; set; } = string.Empty;

    [MaxLength(100, ErrorMessage = "Brand cannot exceed 100 characters")]
    public string? Brand { get; set; }

    [Required(ErrorMessage = "Unit is required")]
    [MaxLength(50, ErrorMessage = "Unit cannot exceed 50 characters")]
    public string Unit { get; set; } = "piece";
}
