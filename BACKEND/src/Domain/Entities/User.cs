using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class User : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Surname { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string UserRole { get; set; } = "User";

    // Navigation Properties
    public ICollection<UserAddress> Addresses { get; set; } = new List<UserAddress>();
    public ICollection<ShoppingList> ShoppingLists { get; set; } = new List<ShoppingList>();
}
