using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class UserAddress : BaseEntity
{
    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(500)]
    public string Address { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string District { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string PostalCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Country { get; set; } = string.Empty;

    // Navigation Property
    public User User { get; set; } = null!;
}
