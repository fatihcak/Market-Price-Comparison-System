using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

/// <summary>
/// Represents an administrator user
/// </summary>
public class AdminUser : BaseEntity
{
    /// <summary>
    /// Admin username (unique)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// BCrypt hashed password
    /// </summary>
    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Last successful login timestamp
    /// </summary>
    public DateTime? LastLogin { get; set; }
}


