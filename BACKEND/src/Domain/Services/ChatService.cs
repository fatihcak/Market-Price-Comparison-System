using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Runtime.CompilerServices;
using Domain.Entities;
using DTOs.DTOs.Responses;
using Microsoft.Extensions.Caching.Memory;

namespace Domain.Services
{
    public class ChatService : IChatService
    {
        private readonly IGeminiApiClient _geminiClient;
        private readonly IProductRepository _productRepository;
        private readonly IMarketRepository _marketRepository;
        private readonly IPriceRepository _priceRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<ChatService> _logger;

        public ChatService(
            IGeminiApiClient geminiClient,
            IProductRepository productRepository,
            IMarketRepository marketRepository,
            IPriceRepository priceRepository,
            IMemoryCache memoryCache,
            ILogger<ChatService> logger)
        {
            _geminiClient = geminiClient;
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
                            
                            // Batch search first (1 DB call for all items)
                            var batchMatches = (await _productRepository.SearchByNamesAsync(analysis.Items)).ToList();
                            var itemsNeedingFuzzy = new List<string>();
                            
                            // First pass: exact match from batch
                            foreach (var item in analysis.Items)
                            {
                                var match = batchMatches.FirstOrDefault(p => 
                                    p.ProductName.Contains(item, StringComparison.OrdinalIgnoreCase));
                                
                                if (match != null)
                                {
                                    shoppingList.Add(match.ProductName);
                                    addedItems.Add(match.ProductName);
                                }
                                else
                                {
                                    itemsNeedingFuzzy.Add(item);
                                }
                            }
                            
                            // Second pass: parallel fuzzy search for unmatched
                            if (itemsNeedingFuzzy.Any())
                            {
                                var fuzzyTasks = itemsNeedingFuzzy.Select(async item =>
                                {
                                    var fuzzyMatches = await _productRepository.SearchWithFuzzyAsync(item);
                                    return (Item: item, Match: fuzzyMatches.FirstOrDefault());
                                });
                                
                                var fuzzyResults = await Task.WhenAll(fuzzyTasks);
                                
                                foreach (var result in fuzzyResults)
                                {
                                    var canonicalName = result.Match?.ProductName ?? result.Item;
                                    shoppingList.Add(canonicalName);
                                    addedItems.Add(canonicalName);
                                }
                            }
                            
                            _memoryCache.Set(shoppingListKey, shoppingList, TimeSpan.FromMinutes(60));
                            response.Reply = $"Added {string.Join(", ", addedItems)} to your list.";
                            
                            // Also return the added products for frontend to add to its cart
                            var allMatchedProducts = batchMatches.ToList();
                            var pIds = allMatchedProducts.Select(p => p.Id).Distinct();
                            var pPrices = await _priceRepository.GetPricesForProductsAsync(pIds);
                            var allMarketsForProducts = await _marketRepository.GetAllAsync();
                            
                            response.FoundProducts = allMatchedProducts
                                .Where(p => addedItems.Contains(p.ProductName))
                                .Take(15)
                                .Select(p =>
                                {
                                    var priceInfo = pPrices.Where(pr => pr.ProductId == p.Id).OrderBy(pr => pr.Price).FirstOrDefault();
                                    var marketName = priceInfo != null 
                                        ? allMarketsForProducts.FirstOrDefault(m => m.Id == priceInfo.MarketId)?.MarketName ?? "Unknown"
                                        : "N/A";
                                    return new ChatProductDto
                                    {
                                        Id = p.Id,
                                        Name = p.ProductName,
                                        Price = priceInfo?.Price ?? 0,
                                        Market = marketName,
                                        Category = p.Category?.CategoryName,
                                        ImageUrl = p.ImageUrl
                                    };
                                })
                                .ToList();
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
                        // Redirect to chat/RAG - it already handles price queries well
                        goto default;

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
                                // ADVANCED RAG: Score and rank products by relevance
                                var scoredProducts = foundAllProducts
                                    .DistinctBy(p => p.Id)
                                    .Select(p => new
                                    {
                                        Product = p,
                                        Score = RelevanceScoring.CalculateRelevanceScore(userMessage, p)
                                    })
                                    .OrderByDescending(x => x.Score)
                                    .Take(25) // Increased for better context
                                    .Select(x => x.Product)
                                    .ToList();

                                // 1. Get prices for context
                                var productIds = scoredProducts.Select(p => p.Id).Distinct();
                                var prices = await _priceRepository.GetPricesForProductsAsync(productIds);
                                
                                // 2. Build Context String
                                var sb = new StringBuilder();
                                sb.AppendLine("System Info (Real Database Data):");
                                foreach (var p in scoredProducts)
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

                                var ragResponse = await _geminiClient.SendRequestAsync(ragPrompt);
                                
                                try 
                                {
                                     var json = ragResponse.Replace("```json", "").Replace("```", "").Trim();
                                     using var doc = JsonDocument.Parse(json);
                                     
                                     // Try to get "reply" first (our format)
                                     if (doc.RootElement.TryGetProperty("reply", out var replyProp))
                                     {
                                         reply = replyProp.GetString();
                                     }
                                     // Fallback: try "response" (sometimes AI uses this)
                                     else if (doc.RootElement.TryGetProperty("response", out var responseProp))
                                     {
                                         reply = responseProp.GetString();
                                     }
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogWarning(ex, "RAG response parsing failed. Raw: {Response}", ragResponse);
                                    // Fallback: use original reply or raw response
                                    // Don't show JSON to user
                                }
                            }
                            else
                            {
                                // No products found in DB
                                reply = "I checked our database, but I couldn't find any products matching your request (tried exact name, similar names, and categories).";
                            }
                        }

                        // Clean up JSON from reply if present
                        if (!string.IsNullOrEmpty(reply))
                        {
                            try
                            {
                                var json = reply.Replace("```json", "").Replace("```", "").Trim();
                                // Check if it looks like JSON
                                if (json.StartsWith("{") && json.EndsWith("}"))
                                {
                                    using var doc = JsonDocument.Parse(json);
                                    
                                    if (doc.RootElement.TryGetProperty("reply", out var replyProp))
                                    {
                                        reply = replyProp.GetString();
                                    }
                                    else if (doc.RootElement.TryGetProperty("response", out var responseProp))
                                    {
                                        reply = responseProp.GetString();
                                    }
                                }
                            }
                            catch
                            {
                                // Not JSON or can't parse, use as-is
                            }
                        }

                        // For 'chat' intent, use the reply generated (or RAG updated)
                        response = new ChatResponseDto { Reply = reply ?? "I'm sorry, I didn't understand that." };
                        
                        // Add found products to response for frontend cart integration
                        if (analysis.Items != null && analysis.Items.Any())
                        {
                            var productsToAdd = new List<Product>();
                            
                            // Search for EACH item separately to ensure all items are found
                            foreach (var item in analysis.Items)
                            {
                                // 1. Try exact match first
                                var exactMatches = await _productRepository.SearchByNamesAsync(new[] { item });
                                if (exactMatches.Any())
                                {
                                    productsToAdd.AddRange(exactMatches);
                                }
                                else
                                {
                                    // 2. Fuzzy search if exact match fails
                                    var fuzzy = await _productRepository.SearchWithFuzzyAsync(item);
                                    productsToAdd.AddRange(fuzzy);
                                }
                            }
                            
                            if (productsToAdd.Any())
                            {
                                var pIds = productsToAdd.Select(p => p.Id).Distinct();
                                var pPrices = await _priceRepository.GetPricesForProductsAsync(pIds);
                                var allMarketsForProducts = await _marketRepository.GetAllAsync();
                                
                                response.FoundProducts = productsToAdd
                                    .DistinctBy(p => p.Id)
                                    .Select(p =>
                                    {
                                        var priceInfo = pPrices.Where(pr => pr.ProductId == p.Id).OrderBy(pr => pr.Price).FirstOrDefault();
                                        var marketName = priceInfo != null 
                                            ? allMarketsForProducts.FirstOrDefault(m => m.Id == priceInfo.MarketId)?.MarketName ?? "Unknown"
                                            : "N/A";
                                        return new ChatProductDto
                                        {
                                            Id = p.Id,
                                            Name = p.ProductName,
                                            Price = priceInfo?.Price ?? 0,
                                            Market = marketName,
                                            Category = p.Category?.CategoryName,
                                            ImageUrl = p.ImageUrl
                                        };
                                    })
                                    .OrderBy(p => p.Price) // Sort by price (cheapest first)
                                    .Take(25) // Show more products (was 5)
                                    .ToList();
                                
                                // Don't cache responses with products (so frontend always gets fresh data)
                                isCacheable = false;
                            }
                        }
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
                
                // Also keep FoundProducts if present
                if (response.FoundProducts != null && response.FoundProducts.Any())
                {
                    if (lastResponse == null) lastResponse = response;
                    else lastResponse.FoundProducts = response.FoundProducts;
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

        public async IAsyncEnumerable<string> GetChatResponseStreamAsync(
            string userMessage, 
            string sessionId,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            // Retrieve history
            var historyKey = $"chat_history_{sessionId}";
            var history = _memoryCache.Get<List<string>>(historyKey) ?? new List<string>();

            // Analyze intent first (non-streaming)
            var analysisResults = await AnalyzeUserMessageAsync(userMessage, history);
            var firstAnalysis = analysisResults.FirstOrDefault();

            if (firstAnalysis == null)
            {
                yield return "{\"chunk\": \"I'm sorry, I couldn't understand that.\"}";
                yield break;
            }

            // For non-chat intents, handle synchronously and stream result
            if (firstAnalysis.Intent != "chat")
            {
                var response = await GetChatResponseAsync(userMessage, sessionId);
                yield return $"{{\"chunk\": \"{EscapeJson(response.Reply)}\"}}";
                yield break;
            }

            // For chat intent: stream the AI response
            // Get real markets from database (materialize to list first)
            var allMarkets = (await _marketRepository.GetAllAsync()).ToList();
            var marketNames = allMarkets.Any() 
                ? string.Join(", ", allMarkets.Select(m => m.MarketName))
                : "No markets available";
            
            // --- RAG-LITE: Search for products if items were mentioned ---
            var contextString = "";
            if (firstAnalysis.Items != null && firstAnalysis.Items.Any())
            {
                var foundProducts = new List<Product>();
                
                // 1. Exact search
                var exactMatches = await _productRepository.SearchByNamesAsync(firstAnalysis.Items);
                foundProducts.AddRange(exactMatches);
                
                // 2. Fuzzy search if no exact matches
                if (!foundProducts.Any())
                {
                    foreach (var item in firstAnalysis.Items)
                    {
                        var fuzzyMatches = await _productRepository.SearchWithFuzzyAsync(item);
                        foundProducts.AddRange(fuzzyMatches);
                    }
                }
                
                // 3. Category search as fallback
                if (!foundProducts.Any())
                {
                    foreach (var item in firstAnalysis.Items)
                    {
                        var catMatches = await _productRepository.SearchByCategoryAsync(item);
                        foundProducts.AddRange(catMatches);
                    }
                }
                
                if (foundProducts.Any())
                {
                    // Score and rank products
                    var scoredProducts = foundProducts
                        .DistinctBy(p => p.Id)
                        .Select(p => new { Product = p, Score = RelevanceScoring.CalculateRelevanceScore(userMessage, p) })
                        .OrderByDescending(x => x.Score)
                        .Take(15)
                        .Select(x => x.Product)
                        .ToList();
                    
                    // Get prices
                    var productIds = scoredProducts.Select(p => p.Id).Distinct();
                    var prices = await _priceRepository.GetPricesForProductsAsync(productIds);
                    
                    // Build context
                    var sb = new StringBuilder();
                    sb.AppendLine("\n=== REAL DATABASE PRODUCTS (USE THIS DATA!) ===");
                    foreach (var p in scoredProducts)
                    {
                        var priceInfo = prices.Where(pr => pr.ProductId == p.Id).OrderBy(pr => pr.Price).FirstOrDefault();
                        var priceStr = priceInfo != null ? $"{priceInfo.Price} TL" : "Price N/A";
                        var marketName = priceInfo != null ? allMarkets.FirstOrDefault(m => m.Id == priceInfo.MarketId)?.MarketName ?? "Unknown" : "N/A";
                        sb.AppendLine($"- {p.ProductName} | Category: {p.Category?.CategoryName ?? "N/A"} | Price: {priceStr} at {marketName}");
                    }
                    sb.AppendLine("=== END OF DATABASE PRODUCTS ===\n");
                    contextString = sb.ToString();
                }
            }
            // --- END RAG-LITE ---
            
            var prompt = $@"
            You are a Market Price Comparison Assistant for Turkish supermarkets. Your ONLY role is to help users with:
            - Product prices and availability
            - Market comparisons
            - Shopping lists
            - Smart basket calculations
            - Product recommendations from our database

            AVAILABLE MARKETS IN OUR SYSTEM:
            {marketNames}
            
            IMPORTANT: ONLY mention the markets listed above. NEVER invent or mention markets that are not in this list!

            IMPORTANT RULES:
            - NEVER answer general knowledge questions (history, science, celebrities, etc.)
            - If asked about non-market topics, politely redirect: ""I'm a market assistant. I can only help with product prices and shopping. What would you like to know about our markets?""
            - Only discuss products, prices, markets, and shopping
            - Be helpful and friendly, but stay in your role
            - When asked about markets, ONLY mention: {marketNames}
            - If DATABASE PRODUCTS are provided below, USE THEM to answer with real prices!

            {contextString}

            Previous conversation history:
            {string.Join("\n", history)}

            User Message: {userMessage}

            Generate a helpful response. If database products are provided, list them with their prices!
            ";

            var fullResponse = new StringBuilder();
            
            await foreach (var chunk in _geminiClient.SendStreamingRequestAsync(prompt, cancellationToken))
            {
                fullResponse.Append(chunk);
                yield return $"{{\"chunk\": \"{EscapeJson(chunk)}\"}}";
            }

            // Update history after streaming completes
            history.Add($"User: {userMessage}");
            history.Add($"Assistant: {fullResponse}");
            
            if (history.Count > 10)
            {
                history = history.Skip(history.Count - 10).ToList();
            }
            
            _memoryCache.Set(historyKey, history, TimeSpan.FromMinutes(30));
        }

        private string EscapeJson(string text)
        {
            return text.Replace("\\", "\\\\")
                       .Replace("\"", "\\\"")
                       .Replace("\n", "\\n")
                       .Replace("\r", "\\r")
                       .Replace("\t", "\\t");
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
               - If the user explicitly asks to CREATE A SHOPPING BASKET, CALCULATE TOTAL COST, or COMPARE MARKET PRICES for multiple items using keywords like 'basket', 'sepet', 'total', 'toplam', 'compare markets', 'en ucuz market', set intent to 'smart_basket'.
               - If the user wants to ADD items to their current shopping list (e.g., 'Add milk', 'Also eggs', 'Listeye ekle'), set intent to 'add_to_list'.
               - If the user wants to REMOVE items (e.g., 'Remove milk', 'Listeden çıkar'), set intent to 'remove_from_list'.
               - If the user asks to SEE their list (e.g., 'What is in my list?', 'Show list', 'Listem'), set intent to 'show_list'.
               - If the user wants to CLEAR/EMPTY the list (e.g., 'Clear list', 'Listeyi temizle'), set intent to 'clear_list'.
               - If the user wants to CALCULATE/FIND CHEAPEST MARKET for the accumulated list (e.g., 'Calculate now', 'Find best price', 'Hesapla'), set intent to 'calculate_list'.
               - If the user simply asks about products, prices, or lists items WITHOUT asking for basket calculation (e.g., 'milk and eggs', 'yumurta', 'show me bread prices'), set intent to 'chat'. This is for browsing products.
               - If it's a general question (e.g., 'Do you have milk?', 'Price of tomato'), set intent to 'chat'.


            2. Extract Information:
               - Extract product names/items from the message.
               - IMPORTANT: Include BOTH English AND Turkish versions of product names to search our database comprehensively:
                 * milk → include both 'sut' and 'milk'
                 * eggs, egg → include both 'yumurta' and 'egg'
                 * bread → include both 'ekmek' and 'bread'
                 * cheese → include both 'peynir' and 'cheese'
                 * butter → include both 'tereyagi' and 'butter'
                 * tomato → include both 'domates' and 'tomato'
                 * water → include both 'su' and 'water'
                 * sugar → include both 'seker' and 'sugar'
                 * oil, olive oil → include both 'zeytinyagi' and 'oil'
                 * chicken → include both 'tavuk' and 'chicken'
                 * meat → include both 'et' and 'meat'
                 * rice → include both 'pirinc' and 'rice'
                 * pasta → include both 'makarna' and 'pasta'
                 * coffee → include both 'kahve' and 'coffee'
                 * tea → include both 'cay' and 'tea'
                 * juice → include both 'meyve suyu' and 'juice'
                 * yogurt → include 'yogurt'
                 * chocolate → include both 'cikolata' and 'chocolate'
               - Example: 'Do you have milk?' -> items: ['sut', 'milk']
               - Example: 'Add eggs and bread' -> items: ['yumurta', 'egg', 'ekmek', 'bread']
            
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

            var response = await _geminiClient.SendRequestAsync(prompt);
            
            try 
            {
                var json = response.Replace("```json", "").Replace("```", "").Trim();
                
                // Handle potential single object response from AI by wrapping in array if needed
                if (json.StartsWith("{"))
                {
                    json = $"[{json}]";
                }

                var results = JsonSerializer.Deserialize<List<MessageAnalysisResult>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                if (results == null || !results.Any())
                {
                    _logger.LogWarning("Intent analysis returned null or empty. Raw response: {Response}", response);
                    return new List<MessageAnalysisResult> { new MessageAnalysisResult { Intent = "chat", Reply = "I'm sorry, I'm having trouble understanding. Could you rephrase that?" } };
                }
                
                return results;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error parsing AI intent analysis. Raw response: {Response}", response);
                
                // If parsing fails, return a safe default
                return new List<MessageAnalysisResult> 
                { 
                    new MessageAnalysisResult 
                    { 
                        Intent = "chat", 
                        Reply = "I apologize, I'm experiencing some technical difficulties. Please try rephrasing your message or use simpler terms."
                    } 
                };
            }
        }

        private async Task<ChatResponseDto> CalculateSmartBasketAsync(List<string> items)
        {
            // 1. Batch fetch all relevant products for the requested items (1 DB call)
            var allFoundProducts = (await _productRepository.SearchByNamesAsync(items)).ToList();
            
            var productMap = new Dictionary<string, List<Product>>();
            var allProductIds = new List<int>();
            var itemsNeedingFuzzy = new List<string>();

            // First pass: Try exact matching from batch results
            foreach (var item in items)
            {
                var exactMatches = allFoundProducts
                    .Where(p => p.ProductName.Contains(item, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                
                if (exactMatches.Any())
                {
                    productMap[item] = exactMatches;
                    allProductIds.AddRange(exactMatches.Select(p => p.Id));
                }
                else
                {
                    // Mark for fuzzy search
                    itemsNeedingFuzzy.Add(item);
                }
            }

            // Second pass: Batch fuzzy search for items without exact matches
            // Note: SearchWithFuzzyAsync handles one term at a time but is already optimized
            // For true batching, we'd need a new repository method, but we can parallelize
            if (itemsNeedingFuzzy.Any())
            {
                // Run fuzzy searches in parallel (reduces wall-clock time)
                var fuzzyTasks = itemsNeedingFuzzy.Select(async item => 
                {
                    var fuzzyMatches = await _productRepository.SearchWithFuzzyAsync(item);
                    return (Item: item, Matches: fuzzyMatches.ToList());
                });
                
                var fuzzyResults = await Task.WhenAll(fuzzyTasks);
                
                foreach (var result in fuzzyResults)
                {
                    if (result.Matches.Any())
                    {
                        productMap[result.Item] = result.Matches;
                        allProductIds.AddRange(result.Matches.Select(p => p.Id));
                        itemsNeedingFuzzy.Remove(result.Item);
                    }
                }
            }

            // Third pass: Category search for remaining items (parallel)
            if (itemsNeedingFuzzy.Any())
            {
                var categoryTasks = itemsNeedingFuzzy.Select(async item =>
                {
                    var catMatches = await _productRepository.SearchByCategoryAsync(item);
                    return (Item: item, Matches: catMatches.ToList());
                });
                
                var categoryResults = await Task.WhenAll(categoryTasks);
                
                foreach (var result in categoryResults)
                {
                    if (result.Matches.Any())
                    {
                        productMap[result.Item] = result.Matches;
                        allProductIds.AddRange(result.Matches.Select(p => p.Id));
                    }
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


        private class MessageAnalysisResult
        {
            public string Intent { get; set; } = "chat";
            public List<string> Items { get; set; } = new List<string>();
            public string? Reply { get; set; }
        }
    }
}
