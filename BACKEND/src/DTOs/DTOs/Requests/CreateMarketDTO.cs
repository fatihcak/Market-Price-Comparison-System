using System.ComponentModel.DataAnnotations;

namespace DTOs.DTOs.Requests;

public class CreateMarketDTO
{
    [Required(ErrorMessage = "Market name is required")]
    [MaxLength(200, ErrorMessage = "Market name cannot exceed 200 characters")]
    public string MarketName { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Logo URL cannot exceed 500 characters")]
    [Url(ErrorMessage = "Logo URL must be a valid URL")]
    public string? LogoUrl { get; set; }

    [MaxLength(500, ErrorMessage = "Website URL cannot exceed 500 characters")]
    [Url(ErrorMessage = "Website URL must be a valid URL")]
    public string? Website { get; set; }
}
