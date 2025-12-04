namespace DTOs.DTOs.Responses;

public class UserProductListDTO
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductImage { get; set; }
    public int Quantity { get; set; }
    public decimal? EstimatedPrice { get; set; } // Average price for display
}
