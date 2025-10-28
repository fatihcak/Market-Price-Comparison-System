using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class ShoppingListItem : BaseEntity
{
    [Required]
    public int ShoppingListId { get; set; }

    [Required]
    public int ProductId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    // Navigation Properties
    public ShoppingList ShoppingList { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
