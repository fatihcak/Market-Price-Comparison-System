using API.Constants;
using Domain.Interfaces.Services;
using Microsoft.Extensions.Caching.Memory;
using DTOs.DTOs.Responses;

namespace API.Services;

public interface ICacheWarmer
{
    Task WarmupAsync();
}

public class CacheWarmer : ICacheWarmer
{
    private readonly IMemoryCache _cache;
    private readonly IProductService _productService;
    private readonly IMarketService _marketService;

    public CacheWarmer(IMemoryCache cache, IProductService productService, IMarketService marketService, IServiceProvider serviceProvider) 
    {
        _cache = cache;
        _productService = productService;
        _marketService = marketService;
        // Injecting IServiceProvider to resolve scoped services if needed
    }

    public async Task WarmupAsync()
    {
        Console.WriteLine("🔥 Warming up cache...");

        try 
        {
            // 1. Discounted Products
            var (discounted, _) = await _productService.GetProductsOrderedByDiscountAsync(1, 100);
            _cache.Set(CacheKeys.DiscountedProducts, discounted, TimeSpan.FromHours(6));
             Console.WriteLine("  ✓ Discounted products cached (Top 100)");

            // 2. Categories (First Pages)
            // IDs 1 to 7 based on the constant list user provided in prompt/file
            int[] categoryIds = { 1, 2, 3, 4, 5, 6, 7 }; 
            int processed = 0;
            foreach (var catId in categoryIds)
            {
                var products = await _productService.GetProductsByCategoryAsync(catId);
                // Note: Taking top 50 manually as service currently returns all
                var top50 = products.Take(50).ToList();
                _cache.Set(CacheKeys.CategoryPage(catId), top50, TimeSpan.FromHours(6));
                processed++;
            }
             Console.WriteLine($"  ✓ Categories cached ({processed}/7)");

            // 3. Markets
            _cache.Set(CacheKeys.AllMarkets, await _marketService.GetAllMarketsAsync(), TimeSpan.FromHours(24));
             Console.WriteLine("  ✓ Markets cached (24h)");
            
            Console.WriteLine("✅ Cache ready! Server is blazing fast 🚀");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Cache warmup failed: {ex.Message}");
        }
    }
}
