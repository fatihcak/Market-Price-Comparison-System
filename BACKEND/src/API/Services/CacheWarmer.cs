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
        try
        {
            // 1. Discounted Products
            var (discounted, _) = await _productService.GetProductsOrderedByDiscountAsync(1, 100);
            _cache.Set(CacheKeys.DiscountedProducts, discounted, TimeSpan.FromHours(6));

            // 2. Categories (First Pages)
            // UPDATED: Use correct backend category IDs (15-21)
            // 15: Fruits & Vegetables, 16: Meat/Chicken/Fish, 17: Dairy/Breakfast
            // 18: Staple Food, 19: Drink, 20: Snacks/Dessert, 21: Cleaning/Personal Care
            int[] categoryIds = { 15, 16, 17, 18, 19, 20, 21 };
            int processed = 0;
            foreach (var catId in categoryIds)
            {
                var products = await _productService.GetProductsByCategoryAsync(catId);
                // Note: Taking top 50 manually as service currently returns all
                var top50 = products.Take(50).ToList();
                _cache.Set(CacheKeys.CategoryPage(catId), top50, TimeSpan.FromHours(6));
                processed++;
            }

            // 3. Markets
            _cache.Set(CacheKeys.AllMarkets, await _marketService.GetAllMarketsAsync(), TimeSpan.FromHours(24));
        }
        catch
        {
            // Cache warmup failed silently - not critical
        }
    }
}
