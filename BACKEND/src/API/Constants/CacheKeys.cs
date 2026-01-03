namespace API.Constants;

public static class CacheKeys
{
    public const string DiscountedProducts = "discounted_products";
    public static string CategoryPage(int categoryId) => $"category_{categoryId}_page_1";
    public const string AllMarkets = "all_markets";
}
