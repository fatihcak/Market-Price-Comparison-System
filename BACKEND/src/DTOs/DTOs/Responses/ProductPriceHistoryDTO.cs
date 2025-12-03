namespace DTOs.DTOs.Responses;

public class ProductPriceHistoryDTO
{
    public DateTime Date { get; set; }
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public decimal AveragePrice { get; set; }
}
