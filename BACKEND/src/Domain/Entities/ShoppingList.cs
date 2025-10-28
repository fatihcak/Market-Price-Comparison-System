using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class ShoppingList : BaseEntity
{
    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(200)]
    public string ListName { get; set; } = string.Empty;

    // Navigation Properties
    public User User { get; set; } = null!;
    public ICollection<ShoppingListItem> Items { get; set; } = new List<ShoppingListItem>();
}
