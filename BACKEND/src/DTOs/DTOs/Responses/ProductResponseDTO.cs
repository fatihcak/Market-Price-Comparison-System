namespace DTOs.DTOs.Responses;

public class ProductResponseDTO
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? Brand { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? OldPrice { get; set; }
    public int Discount { get; set; }
    public string MarketName { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ImageUrl { get; set; }
}
