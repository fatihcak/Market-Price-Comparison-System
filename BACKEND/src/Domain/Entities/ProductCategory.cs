using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class ProductCategory : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string CategoryName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Icon { get; set; }

    // Navigation Property
    public ICollection<Product> Products { get; set; } = new List<Product>();
}