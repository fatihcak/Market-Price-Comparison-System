namespace API.Constants;

public static class CacheKeys
{
    public const string DiscountedProducts = "discounted_products";
    public static string CategoryPage(int categoryId) => $"category_{categoryId}_page_1";
    public const string AllMarkets = "all_markets";
    public static string ProductById(int productId) => $"product_{productId}";
    public static string SearchResults(string query) => $"search_{query.ToLowerInvariant().Trim()}";
}
