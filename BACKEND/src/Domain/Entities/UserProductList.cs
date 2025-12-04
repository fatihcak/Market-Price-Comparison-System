using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class UserProductList : BaseEntity
{
    [Required]
    [MaxLength(255)]
    public string SessionId { get; set; } = string.Empty;

    [Required]
    public int ProductId { get; set; }

    [Required]
    public int Quantity { get; set; }

    public DateTime AddedDate { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public Product Product { get; set; } = null!;
}
