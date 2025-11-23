using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Market : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string MarketName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? LogoUrl { get; set; }

    [MaxLength(500)]
    public string? Website { get; set; }

    // Navigation Property
    public ICollection<MarketProductPrice> MarketProductPrices { get; set; } = new List<MarketProductPrice>();
}
