using System.ComponentModel.DataAnnotations;

namespace DTOs.DTOs.Requests;

public class CreateShoppingListDTO
{
    [Required]
    public string SessionId { get; set; } = string.Empty;

    [Required]
    public int ProductId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }
}
