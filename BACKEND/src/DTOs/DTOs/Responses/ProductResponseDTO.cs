namespace DTOs.DTOs.Responses;

public class ProductResponseDTO
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? Brand { get; set; }
    public string? Unit { get; set; }
    public decimal Price { get; set; }
    public decimal? OldPrice { get; set; }
    public int Discount { get; set; }
    public string MarketName { get; set; } = string.Empty;
    public int MarketCount { get; set; } // Number of unique markets selling this product
    public List<string> AllMarketNames { get; set; } = new(); // Names of all markets selling this product
    public DateTime? LastUpdated { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? ImageUrl { get; set; }
}
