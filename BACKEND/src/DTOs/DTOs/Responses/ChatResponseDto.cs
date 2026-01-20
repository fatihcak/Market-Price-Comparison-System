using System.Collections.Generic;

namespace DTOs.DTOs.Responses
{
    public class ChatResponseDto
    {
        public string Reply { get; set; } = string.Empty;
        public BasketSuggestionDto? BasketSuggestion { get; set; }
        public List<ChatProductDto> FoundProducts { get; set; } = new();
    }

    public class ChatProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Market { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? ImageUrl { get; set; }
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
