using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Price : BaseEntity
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    public int MarketId { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal PriceValue { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? DiscountPrice { get; set; }

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public Product Product { get; set; } = null!;
    public Market Market { get; set; } = null!;
}
