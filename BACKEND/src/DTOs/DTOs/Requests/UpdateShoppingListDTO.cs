using System.ComponentModel.DataAnnotations;

namespace DTOs.DTOs.Requests;

public class UpdateShoppingListDTO
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }
}
