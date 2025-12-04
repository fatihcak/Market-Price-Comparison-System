namespace DTOs.DTOs.Responses;

public class ShoppingListResponseDTO
{
    public int Id { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTime AddedDate { get; set; }
}
