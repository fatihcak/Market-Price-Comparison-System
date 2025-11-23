using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class City : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string CityName { get; set; } = string.Empty;

    // Navigation Properties
    public ICollection<District> Districts { get; set; } = new List<District>();
}
