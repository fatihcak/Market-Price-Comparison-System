namespace DTOs.DTOs.Responses;

public class PriceResponseDTO
{
    public int Id { get; set; }
    public int MarketId { get; set; }
    public string MarketName { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int DistrictId { get; set; }
    public string DistrictName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime LastUpdated { get; set; }
}
