using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.Entities;
using DTOs.DTOs.Responses;
using DTOs.DTOs.Requests;

namespace Domain.Services
{
    public class ChatService : IChatService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private readonly IProductRepository _productRepository;
        private readonly IMarketRepository _marketRepository;
        private readonly IPriceRepository _priceRepository;

        public ChatService(
            IConfiguration configuration, 
            HttpClient httpClient,
            IProductRepository productRepository,
            IMarketRepository marketRepository,
            IPriceRepository priceRepository)
        {
            _apiKey = configuration["AiSettings:GoogleApiKey"]!;
            _httpClient = httpClient;
            _productRepository = productRepository;
            _marketRepository = marketRepository;
            _priceRepository = priceRepository;
        }

        public async Task<ChatResponseDto> GetChatResponseAsync(string userMessage)
        {
            var analysis = await AnalyzeUserMessageAsync(userMessage);

            if (analysis.Intent == "smart_basket" && analysis.Items != null && analysis.Items.Any())
            {
                return await CalculateSmartBasketAsync(analysis.Items);
            }

            var simpleReply = await GetSimpleChatResponseAsync(userMessage);
            return new ChatResponseDto { Reply = simpleReply };
        }

        private async Task<MessageAnalysisResult> AnalyzeUserMessageAsync(string message)
        {
            var prompt = $@"
            Analyze the following user message: '{message}'.
            Identify the user's intent. 
            If the user wants to buy products or asks for a price comparison for multiple items, set intent to 'smart_basket'. Extract the list of items. IMPORTANT: Keep the item names in the original language used by the user (e.g., if user says 'tomato', extract 'tomato'; if 'domates', extract 'domates').
            
            If it's a general question, set intent to 'chat'.
            
            Return ONLY a JSON object in this format:
            {{
                ""intent"": ""smart_basket"" or ""chat"",
                ""items"": [""item1"", ""item2""]
            }}
            ";

            var response = await CallGeminiApiAsync(prompt);
            
            try 
            {
                var json = response.Replace("```json", "").Replace("```", "").Trim();
                return JsonSerializer.Deserialize<MessageAnalysisResult>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) 
                       ?? new MessageAnalysisResult { Intent = "chat" };
            }
            catch
            {
                return new MessageAnalysisResult { Intent = "chat" };
            }
        }

        private async Task<ChatResponseDto> CalculateSmartBasketAsync(List<string> items)
        {
            // 1. Batch fetch all relevant products for the requested items
            var allFoundProducts = await _productRepository.SearchByNamesAsync(items);
            
            var productMap = new Dictionary<string, List<Product>>();
            var allProductIds = new List<int>();

            // Map products back to the requested items (in-memory)
            foreach (var item in items)
            {
                // We use the same 'Contains' logic as the repository to ensure consistency
                var matches = allFoundProducts
                    .Where(p => p.ProductName.Contains(item, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (matches.Any())
                {
                    productMap[item] = matches;
                    allProductIds.AddRange(matches.Select(p => p.Id));
                }
            }

            if (!allProductIds.Any())
            {
                return new ChatResponseDto 
                { 
                    Reply = "Sorry, we could not find any records for the requested products in our database." 
                };
            }

            // 2. Batch fetch prices for all found products
            var allPrices = await _priceRepository.GetPricesForProductsAsync(allProductIds.Distinct());

            // 3. Group prices by Market
            var marketBaskets = new Dictionary<string, MarketBasketDto>();
            var allMarkets = await _marketRepository.GetAllAsync();

            foreach (var market in allMarkets)
            {
                var basket = new MarketBasketDto 
                { 
                    MarketName = market.MarketName, 
                    TotalPrice = 0 
                };
                
                foreach (var item in items)
                {
                    if (!productMap.ContainsKey(item)) continue;

                    var possibleProducts = productMap[item];
                    
                    // Find the cheapest product for this item in this market
                    var bestPrice = allPrices
                        .Where(p => p.MarketId == market.Id && possibleProducts.Any(prod => prod.Id == p.ProductId))
                        .OrderBy(p => p.Price)
                        .FirstOrDefault();

                    if (bestPrice != null)
                    {
                        // We need the product name
                        // Try to find it in possibleProducts (which we have in memory)
                        var product = possibleProducts.FirstOrDefault(p => p.Id == bestPrice.ProductId);
                        var productName = product?.ProductName ?? "Unknown Product";
                        
                        basket.TotalPrice += bestPrice.Price;
                        basket.Items.Add(new ProductItemDto 
                        { 
                            ProductName = productName, 
                            Price = bestPrice.Price 
                        });
                    }
                }

                if (basket.Items.Any())
                {
                    marketBaskets[market.MarketName] = basket;
                }
            }

            if (!marketBaskets.Any())
            {
                 return new ChatResponseDto 
                { 
                    Reply = "Sorry, we could not find any prices for the items you requested." 
                };
            }

            // 4. Determine best market
            var cheapestBasket = marketBaskets.Values.OrderBy(b => b.TotalPrice).First();

            // Calculate missing items for the cheapest basket
            // An item is missing if we didn't add a product for it to the basket
            // We can check this by seeing if any of the products in the basket "match" the item
            // OR simpler: we know which items we found a price for in the loop above.
            
            // Let's re-verify missing items specifically for the cheapest basket
            var missingItems = new List<string>();
            foreach(var item in items)
            {
                // Did we find a price for this 'item' in 'cheapestBasket'?
                // We can check if any product in the basket belongs to the 'productMap[item]' list.
                if (productMap.ContainsKey(item))
                {
                    var possibleProductIds = productMap[item].Select(p => p.Id).ToHashSet();
                    // We need to know the ProductIds in the basket. 
                    // But ProductItemDto only has Name. 
                    // However, we can infer it or check by name again.
                    // Checking by name is safer if we trust the mapping.
                    
                    bool foundInBasket = cheapestBasket.Items.Any(basketItem => 
                        productMap[item].Any(p => p.ProductName == basketItem.ProductName));
                    
                    if (!foundInBasket)
                    {
                        missingItems.Add(item);
                    }
                }
                else
                {
                    // Item itself was not found in DB at all
                    missingItems.Add(item);
                }
            }

            var sb = new StringBuilder();
            sb.AppendLine($"The cheapest basket is in **{cheapestBasket.MarketName}**!");
            sb.AppendLine($"Total Price: **{cheapestBasket.TotalPrice:C2}**");
            
            if (missingItems.Any())
            {
                sb.AppendLine();
                sb.AppendLine("Missing items in this market: " + string.Join(", ", missingItems));
            }

            return new ChatResponseDto
            {
                Reply = sb.ToString(),
                BasketSuggestion = new BasketSuggestionDto
                {
                    CheapestMarketName = cheapestBasket.MarketName,
                    TotalPrice = cheapestBasket.TotalPrice,
                    AlternativeMarkets = marketBaskets.Values.OrderBy(b => b.TotalPrice).Skip(1).Take(3).ToList(),
                    MissingItems = missingItems
                }
            };
        }

        private async Task<string> GetSimpleChatResponseAsync(string message)
        {
            var prompt = $"User says: '{message}'. You are a helpful Market Price Comparison Assistant. Respond in the same language as the user (English or Turkish). Be concise, professional, and do not make up facts. If you don't know something, say so.";
            return await CallGeminiApiAsync(prompt);
        }

        private async Task<string> CallGeminiApiAsync(string prompt)
        {
            // Simple content generation
            var requestBody = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = prompt } } }
                }
            };

            return await ExecuteGeminiRequest(requestBody);
        }

        private async Task<string> ExecuteGeminiRequest(object requestBody)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-pro:generateContent?key={_apiKey}", jsonContent);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Google API Error: {response.StatusCode} - {errorContent}");
                return "AI service is not available at the moment.";
            }

            var responseString = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseString);
            
            try 
            {
                return doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString() ?? "";
            }
            catch
            {
                return "Misunderstood.";
            }
        }

        private class MessageAnalysisResult
        {
            public string Intent { get; set; } = "chat";
            public List<string> Items { get; set; } = new List<string>();
        }
    }
}
