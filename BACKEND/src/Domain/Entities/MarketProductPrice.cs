using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class MarketProductPrice : BaseEntity
{
    public int MarketId { get; set; }

    public int ProductId { get; set; }

    public int DistrictId { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    public DateTime? LastUpdated { get; set; }

    // Navigation Properties
    public Market? Market { get; set; }
    public Product? Product { get; set; }
    public District? District { get; set; }
}
