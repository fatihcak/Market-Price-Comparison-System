using System.ComponentModel.DataAnnotations;

namespace DTOs.DTOs.Requests;

public class UpdateProductDTO
{
    [MaxLength(200, ErrorMessage = "Product name cannot exceed 200 characters")]
    public string? ProductName { get; set; }

    [MaxLength(100, ErrorMessage = "Brand cannot exceed 100 characters")]
    public string? Brand { get; set; }

    [MaxLength(50, ErrorMessage = "Unit cannot exceed 50 characters")]
    public string? Unit { get; set; }
}
