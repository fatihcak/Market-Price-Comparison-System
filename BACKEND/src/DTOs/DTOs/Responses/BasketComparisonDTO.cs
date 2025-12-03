namespace DTOs.DTOs.Responses;

public class BasketComparisonDTO
{
    public string MarketName { get; set; } = string.Empty;
    public string MarketLogoUrl { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public int MissingItemCount { get; set; }
    public List<string> MissingItems { get; set; } = new();
    public bool IsCheapest { get; set; }
}
