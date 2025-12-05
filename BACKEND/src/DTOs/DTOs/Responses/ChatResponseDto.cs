using System.Collections.Generic;

namespace DTOs.DTOs.Responses
{
    public class ChatResponseDto
    {
        public string Reply { get; set; }
        public BasketSuggestionDto? BasketSuggestion { get; set; }
    }

    public class BasketSuggestionDto
    {
        public string CheapestMarketName { get; set; }
        public decimal TotalPrice { get; set; }
        public List<MarketBasketDto> AlternativeMarkets { get; set; } = new();
        public List<string> MissingItems { get; set; } = new();
    }

    public class MarketBasketDto
    {
        public string MarketName { get; set; }
        public decimal TotalPrice { get; set; }
        public List<ProductItemDto> Items { get; set; } = new();
    }

    public class ProductItemDto
    {
        public string ProductName { get; set; }
        public decimal Price { get; set; }
    }
}
