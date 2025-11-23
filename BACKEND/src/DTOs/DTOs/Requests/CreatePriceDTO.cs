using System.ComponentModel.DataAnnotations;

namespace DTOs.DTOs.Requests;

public class CreatePriceDTO
{
    [Required]
    public int MarketId { get; set; }

    [Required]
    public int ProductId { get; set; }

    [Required]
    public int DistrictId { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0")]
    public decimal Price { get; set; }
}
