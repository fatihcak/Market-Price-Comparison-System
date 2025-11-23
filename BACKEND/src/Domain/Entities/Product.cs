using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Product : BaseEntity
{
    [Required]
    public int CategoryId { get; set; }

    [Required]
    [MaxLength(200)]
    public string ProductName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Brand { get; set; }

    [Required]
    [MaxLength(50)]
    public string Unit { get; set; } = "piece";

    [Required]
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public ProductCategory Category { get; set; } = null!;
    public ICollection<MarketProductPrice> MarketProductPrices { get; set; } = new List<MarketProductPrice>();
    public ICollection<UserProductList> UserProductLists { get; set; } = new List<UserProductList>();
}
