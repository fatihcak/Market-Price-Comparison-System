using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class AdminUser : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public DateTime? LastLogin { get; set; }
}
