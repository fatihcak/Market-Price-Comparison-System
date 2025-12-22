using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Product : BaseEntity
{
    public int CategoryId { get; set; }

    [MaxLength(200)]
    public string ProductName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Brand { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [MaxLength(50)]
    public string? Unit { get; set; }

    public DateTime? LastUpdated { get; set; }

    // Navigation Properties
    public ProductCategory? Category { get; set; }
    public ICollection<MarketProductPrice> MarketProductPrices { get; set; } = new List<MarketProductPrice>();
    public ICollection<UserProductList> UserProductLists { get; set; } = new List<UserProductList>();
}
