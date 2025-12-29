using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.Entities;
using DTOs.DTOs.Responses;
using Microsoft.Extensions.Caching.Memory;

namespace Domain.Services
{
    public class ChatService : IChatService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private readonly IProductRepository _productRepository;
        private readonly IMarketRepository _marketRepository;
        private readonly IPriceRepository _priceRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<ChatService> _logger;

        public ChatService(
            IConfiguration configuration, 
            HttpClient httpClient,
            IProductRepository productRepository,
            IMarketRepository marketRepository,
            IPriceRepository priceRepository,
            IMemoryCache memoryCache,
            ILogger<ChatService> logger)
        {
            _apiKey = configuration["AiSettings:GoogleApiKey"]!;
            _httpClient = httpClient;
            _productRepository = productRepository;
            _marketRepository = marketRepository;
            _priceRepository = priceRepository;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task<ChatResponseDto> GetChatResponseAsync(string userMessage, string sessionId)
        {
            // 0. Check Cache (Smart Response Caching)
            // ... (rest of cache logic)

            // ...


            // We create a cache key based on the normalized message.
            // We ONLY return cached responses for general queries, not for state-changing commands.
            // But since we don't know the intent yet, we check a specific "response cache".
            // We will only WRITE to this cache if the resulting intent is cacheable.
            var cacheKey = $"chat_response_{userMessage.Trim().ToLowerInvariant()}";
            if (_memoryCache.TryGetValue(cacheKey, out ChatResponseDto cachedResponse))
            {
                return cachedResponse!;
            }

            // Retrieve history
            var historyKey = $"chat_history_{sessionId}";
            var history = _memoryCache.Get<List<string>>(historyKey) ?? new List<string>();

            // Analyze with history context
            var analysisResults = await AnalyzeUserMessageAsync(userMessage, history);

            var shoppingListKey = $"shopping_list_{sessionId}";
            var shoppingList = _memoryCache.Get<List<string>>(shoppingListKey) ?? new List<string>();

            var combinedReply = new StringBuilder();
            ChatResponseDto lastResponse = null;
            bool isCacheable = true; // Default to true, mark false if any non-cacheable intent is found

            foreach (var analysis in analysisResults)
            {
                ChatResponseDto response = new ChatResponseDto();

                switch (analysis.Intent)
                {
                    case "add_to_list":
                        isCacheable = false; // State change
                        if (analysis.Items != null && analysis.Items.Any())
                        {
                            var addedItems = new List<string>();
                            foreach (var item in analysis.Items)
                            {
                                // Smart Normalization: Try to find the canonical name
                                var fuzzyMatches = await _productRepository.SearchWithFuzzyAsync(item);
                                var canonicalName = fuzzyMatches.FirstOrDefault()?.ProductName ?? item;
                                
                                shoppingList.Add(canonicalName);
                                addedItems.Add(canonicalName);
                            }
                            _memoryCache.Set(shoppingListKey, shoppingList, TimeSpan.FromMinutes(60));
                            response.Reply = $"Added {string.Join(", ", addedItems)} to your list.";
                        }
                        else
                        {
                            response.Reply = "I couldn't identify what to add.";
                        }
                        break;

                    case "remove_from_list":
                        isCacheable = false; // State change
                        if (analysis.Items != null && analysis.Items.Any())
                        {
                            var removedItems = new List<string>();
                            foreach (var item in analysis.Items)
                            {
                                // 1. Try exact match
                                var itemToRemove = shoppingList.FirstOrDefault(i => i.Equals(item, StringComparison.OrdinalIgnoreCase));
                                
                                // 2. Try fuzzy match (Levenshtein Distance <= 3)
                                if (itemToRemove == null)
                                {
                                    // Simple Levenshtein implementation for list matching
                                    itemToRemove = shoppingList
                                        .Select(i => new { Item = i, Distance = StringUtilities.ComputeLevenshteinDistance(item.ToLower(), i.ToLower()) })
                                        .Where(x => x.Distance <= 3)
                                        .OrderBy(x => x.Distance)
                                        .FirstOrDefault()?.Item;
                                }

                                // 3. Try Contains (Fallback)
                                if (itemToRemove == null)
                                {
                                    itemToRemove = shoppingList.FirstOrDefault(i => i.Contains(item, StringComparison.OrdinalIgnoreCase));
                                }

                                if (itemToRemove != null)
                                {
                                    shoppingList.Remove(itemToRemove);
                                    removedItems.Add(itemToRemove);
                                }
                            }
                            _memoryCache.Set(shoppingListKey, shoppingList, TimeSpan.FromMinutes(60));
                            
                            if (removedItems.Any())
                                response.Reply = $"Removed {string.Join(", ", removedItems)} from your list.";
                            else
                                response.Reply = $"Could not find {string.Join(", ", analysis.Items)} in your list.";
                        }
                        break;

                    case "show_list":
                        isCacheable = false; // Depends on list state
                        if (shoppingList.Any())
                        {
                            response.Reply = $"Your shopping list:\n- {string.Join("\n- ", shoppingList)}";
                        }
                        else
                        {
                            response.Reply = "Your shopping list is empty.";
                        }
                        break;

                    case "clear_list":
                        isCacheable = false; // State change
                        shoppingList.Clear();
                        _memoryCache.Remove(shoppingListKey);
                        response.Reply = "Cleared your shopping list.";
                        break;

                    case "calculate_list":
                        isCacheable = false; // Depends on list state
                        if (shoppingList.Any())
                        {
                            response = await CalculateSmartBasketAsync(shoppingList);
                        }
                        else
                        {
                            response.Reply = "Your shopping list is empty. Add some items first!";
                        }
                        break;

                    case "smart_basket":
                        // Cacheable because it depends only on the input items, not external state (mostly)
                        if (analysis.Items != null && analysis.Items.Any())
                        {
                            response = await CalculateSmartBasketAsync(analysis.Items);
                        }
                        else
                        {
                            response.Reply = "Please specify items to calculate the basket for.";
                        }
                        break;

                    case "find_cheaper":
                        // Placeholder for future implementation
                        response.Reply = "The 'Smart Alternatives' feature is currently under maintenance. Please try again later.";
                        break;

                    default: // chat
                        // Cacheable
                        var reply = analysis.Reply;

                        // RAG-Lite: If items were identified in a chat context, look them up and re-generate answer
                        if (analysis.Items != null && analysis.Items.Any())
                        {
                            var foundAllProducts = new List<Product>();
                            
                            // 1. Exact Search
                            var exactMatches = await _productRepository.SearchByNamesAsync(analysis.Items);
                            foundAllProducts.AddRange(exactMatches);

                            // 2. Fuzzy Search (if exact failed for some items)
                            if (!foundAllProducts.Any())
                            {
                                 foreach (var item in analysis.Items)
                                 {
                                     var fuzzyMatches = await _productRepository.SearchWithFuzzyAsync(item);
                                     foundAllProducts.AddRange(fuzzyMatches);
                                 }
                            }

                            // 3. Category Search (if still nothing)
                            if (!foundAllProducts.Any())
                            {
                                foreach (var item in analysis.Items)
                                {
                                    var catMatches = await _productRepository.SearchByCategoryAsync(item);
                                    foundAllProducts.AddRange(catMatches);
                                }
                            }

                            if (foundAllProducts.Any())
                            {
                                // Optimization: Take only top 5 products matching the query to avoid massive price fetch
                                var relevantProducts = foundAllProducts.DistinctBy(p => p.Id).Take(5).ToList();

                                // 1. Get prices for context
                                var productIds = relevantProducts.Select(p => p.Id).Distinct();
                                var prices = await _priceRepository.GetPricesForProductsAsync(productIds);
                                
                                // 2. Build Context String
                                var sb = new StringBuilder();
                                sb.AppendLine("System Info (Real Database Data):");
                                foreach (var p in relevantProducts)
                                {
                                    var priceInfo = prices.Where(pr => pr.ProductId == p.Id).OrderBy(pr => pr.Price).FirstOrDefault();
                                    var priceStr = priceInfo != null ? $"{priceInfo.Price} TL" : "Price N/A";
                                    sb.AppendLine($"- Product: {p.ProductName} (Category: {p.Category?.CategoryName ?? "N/A"}), Best Price: {priceStr}");
                                }
                                
                                var contextString = sb.ToString();

                                // 3. Re-generate response with context
                                var ragPrompt = $@"
                                History:
                                {string.Join("\n", history)}

                                System Context:
                                {contextString}

                                User Message: {userMessage}

                                Instruction: Answer the user's question using the System Context above. 
                                If the user asks for products, list the ones from the System Context.
                                Be helpful and natural.

                                Return ONLY a JSON object in this format:
                                {{
                                    ""reply"": ""Your answer here""
                                }}
                                ";

                                var ragResponse = await CallGeminiApiAsync(ragPrompt);
                                
                                try 
                                {
                                     var json = ragResponse.Replace("```json", "").Replace("```", "").Trim();
                                     using var doc = JsonDocument.Parse(json);
                                     if (doc.RootElement.TryGetProperty("reply", out var replyProp))
                                     {
                                         reply = replyProp.GetString();
                                     }
                                }
                                catch
                                {
                                    // Fallback to original reply if RAG fails
                                }
                            }
                            else
                            {
                                // No products found in DB
                                reply = "I checked our database, but I couldn't find any products matching your request (tried exact name, similar names, and categories).";
                            }
                        }

                        // For 'chat' intent, use the reply generated (or RAG updated)
                        response = new ChatResponseDto { Reply = reply ?? "I'm sorry, I didn't understand that." };
                        break;
                }

                if (!string.IsNullOrEmpty(response.Reply))
                {
                    combinedReply.AppendLine(response.Reply);
                }
                
                // Keep the last response object for BasketSuggestion (if any)
                if (response.BasketSuggestion != null)
                {
                    lastResponse = response;
                }
            }

            // Construct final response
            var finalResponse = lastResponse ?? new ChatResponseDto();
            finalResponse.Reply = combinedReply.ToString().Trim();

            // Cache the response if it's safe to do so
            if (isCacheable && !string.IsNullOrEmpty(finalResponse.Reply))
            {
                _memoryCache.Set(cacheKey, finalResponse, TimeSpan.FromMinutes(5));
            }

            // Update history
            history.Add($"User: {userMessage}");
            if (!string.IsNullOrEmpty(finalResponse.Reply))
            {
                 history.Add($"Assistant: {finalResponse.Reply}");
            }
            
            // Keep only last 10 messages to avoid token limits
            if (history.Count > 10) 
            {
                history = history.Skip(history.Count - 10).ToList();
            }

            _memoryCache.Set(historyKey, history, TimeSpan.FromMinutes(30));

            return finalResponse;
        }

        private async Task<List<MessageAnalysisResult>> AnalyzeUserMessageAsync(string message, List<string> history)
        {
            var historyText = string.Join("\n", history);
            var prompt = $@"
            Previous conversation history:
            {historyText}

            Current user message: '{message}'.
            
            Analyze the message based on the history above.
            
            1. Identify the user's intent(s). The user might want to do multiple things (e.g., 'Add milk and remove bread').
               - If the user explicitly asks to create a basket, comparison, or buy multiple items AND mentions specific products, set intent to 'smart_basket'.
               - If the user wants to ADD items to their current shopping list (e.g., 'Add milk', 'Also eggs', 'Listeye ekle'), set intent to 'add_to_list'.
               - If the user wants to REMOVE items (e.g., 'Remove milk', 'Listeden çıkar'), set intent to 'remove_from_list'.
               - If the user asks to SEE their list (e.g., 'What is in my list?', 'Show list', 'Listem'), set intent to 'show_list'.
               - If the user wants to CLEAR/EMPTY the list (e.g., 'Clear list', 'Listeyi temizle'), set intent to 'clear_list'.
               - If the user wants to CALCULATE/FIND CHEAPEST MARKET for the accumulated list (e.g., 'Calculate now', 'Find best price', 'Hesapla'), set intent to 'calculate_list'.
               - If the user asks to create a list but DOES NOT mention specific products (e.g., 'I want to make a list'), set intent to 'chat' and ask them what they want to add.
               - If it's a general question (e.g., 'Do you have milk?', 'Price of tomato'), set intent to 'chat'.

            2. Extract Information:
               - Example: 'Do you have milk?' -> items: ['milk']
            
            3. Generate Reply (if 'chat'):
               - If intent is 'chat', generate a helpful response.
               - IMPORTANT: If the user asks about product availability or price, give a generic polite response like 'Let me check that for you...' because the system will look it up shortly.
               - If it's just a greeting, respond normally.

            Return ONLY a JSON ARRAY of objects in this format (even if there is only one intent):

            [
                {{
                    ""intent"": ""smart_basket"" or ""chat"" or ""add_to_list"" or ""remove_from_list"" or ""show_list"" or ""clear_list"" or ""calculate_list"" or ""find_cheaper"",
                    ""items"": [""item1"", ""item2""],
                    ""reply"": ""Your response here if intent is chat""
                }}
            ]
            ";

            var response = await CallGeminiApiAsync(prompt);
            
            try 
            {
                var json = response.Replace("```json", "").Replace("```", "").Trim();
                // Handle potential single object response from AI by wrapping in array if needed, 
                // but prompt asks for array.
                if (json.StartsWith("{"))
                {
                    json = $"[{json}]";
                }

                return JsonSerializer.Deserialize<List<MessageAnalysisResult>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) 
                       ?? new List<MessageAnalysisResult> { new MessageAnalysisResult { Intent = "chat", Reply = "Error parsing response (Null result)." } };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error parsing AI response");
                return new List<MessageAnalysisResult> { new MessageAnalysisResult { Intent = "chat", Reply = $"Error parsing AI response. Raw: {response.Substring(0, Math.Min(response.Length, 50))}..." } };
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
                var matches = new List<Product>();

                // 1. Exact Match
                var exactMatches = allFoundProducts
                    .Where(p => p.ProductName.Contains(item, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                matches.AddRange(exactMatches);

                // 2. Fuzzy Match (if no exact match)
                if (!matches.Any())
                {
                    var fuzzyMatches = await _productRepository.SearchWithFuzzyAsync(item);
                    matches.AddRange(fuzzyMatches);
                }

                // 3. Category Match (if still no match)
                if (!matches.Any())
                {
                     var catMatches = await _productRepository.SearchByCategoryAsync(item);
                     matches.AddRange(catMatches);
                }

                if (matches.Any())
                {
                    // Deduplicate
                    matches = matches.DistinctBy(p => p.Id).ToList();
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
            sb.AppendLine($"🏆 Best Option Found!");
            sb.AppendLine($"🛒 Market: {cheapestBasket.MarketName}");
            sb.AppendLine($"💰 Total Price: {cheapestBasket.TotalPrice:N2} TL");
            
            if (missingItems.Any())
            {
                sb.AppendLine();
                sb.AppendLine($"⚠️ Note: The following items were not found in {cheapestBasket.MarketName}: {string.Join(", ", missingItems)}");
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

        private async Task<string> CallGeminiApiAsync(string prompt)
        {
            _logger.LogDebug("Sending request to Google API");
            var requestBody = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = prompt } } }
                },
                generationConfig = new 
                {
                    response_mime_type = "application/json"
                }
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-pro:generateContent?key={_apiKey}", jsonContent);
            
            _logger.LogDebug("Google API Status: {StatusCode}", response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Google API Error: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
                return "AI service is not available at the moment.";
            }

            var responseString = await response.Content.ReadAsStringAsync();
            try
            {
                using var doc = JsonDocument.Parse(responseString);
                return doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString() ?? "";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gemini Parsing Error. Raw Response: {RawResponse}", responseString);
                return "AI Error";
            }
        }

        private class MessageAnalysisResult
        {
            public string Intent { get; set; } = "chat";
            public List<string> Items { get; set; } = new List<string>();
            public string? Reply { get; set; }
        }
    }
}
