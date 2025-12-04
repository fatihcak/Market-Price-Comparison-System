using System.ComponentModel.DataAnnotations;

namespace DTOs.DTOs.Requests;

public class UpdatePriceDTO
{
    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0")]
    public decimal Price { get; set; }
}
