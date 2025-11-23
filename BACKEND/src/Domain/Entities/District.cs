using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class District : BaseEntity
{
    [Required]
    public int CityId { get; set; }

    [Required]
    [MaxLength(100)]
    public string DistrictName { get; set; } = string.Empty;

    // Navigation Properties
    public City City { get; set; } = null!;
    public ICollection<MarketProductPrice> MarketProductPrices { get; set; } = new List<MarketProductPrice>();
}
