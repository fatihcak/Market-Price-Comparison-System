using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class MarketProductPrice : BaseEntity
{
    [Required]
    public int MarketId { get; set; }

    [Required]
    public int ProductId { get; set; }

    [Required]
    public int DistrictId { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Required]
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public Market Market { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public District District { get; set; } = null!;
}
