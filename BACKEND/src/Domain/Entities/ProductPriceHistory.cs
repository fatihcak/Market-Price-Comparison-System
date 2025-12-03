using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class ProductPriceHistory
{
    public int Id { get; set; }

    [Required]
    public int MarketProductPriceId { get; set; }

    [Required]
    public decimal Price { get; set; }

    [Required]
    public DateTime ChangedDate { get; set; } = DateTime.UtcNow;

    // Navigation Property
    public MarketProductPrice MarketProductPrice { get; set; } = null!;
}
