using Domain.Entities;

namespace Domain.Utilities;

public static class RelevanceScoring
{
    public static double CalculateRelevanceScore(string userQuery, Product product)
    {
        var score = 0.0;
        var queryLower = userQuery.ToLowerInvariant();
        var productNameLower = product.ProductName.ToLowerInvariant();
        var brandLower = (product.Brand ?? "").ToLowerInvariant();
        var categoryLower = (product.Category?.CategoryName ?? "").ToLowerInvariant();

        // 1. Exact Match Bonus (highest priority)
        if (productNameLower == queryLower)
            score += 100;
        else if (productNameLower.Contains(queryLower))
            score += 50;

        // 2. Brand Match Bonus
        if (!string.IsNullOrEmpty(product.Brand) && queryLower.Contains(brandLower))
            score += 30;

        // 3. Category Match Bonus
        if (IsCategoryMatch(queryLower, categoryLower))
            score += 25;

        // 4. Fuzzy Match (Levenshtein Distance)
        var distance = StringUtilities.ComputeLevenshteinDistance(queryLower, productNameLower);
        // Score inversely proportional to distance (max 20 points)
        score += Math.Max(0, 20 - distance);

        // 5. Word-level Match Bonus
        var queryWords = queryLower.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var productWords = productNameLower.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var matchingWords = queryWords.Intersect(productWords).Count();
        score += matchingWords * 10;

        return score;
    }

    public static bool IsCategoryMatch(string query, string categoryName)
    {
        if (string.IsNullOrEmpty(categoryName))
            return false;

        var categoryLower = categoryName.ToLowerInvariant();

        // Direct category name match
        if (query.Contains(categoryLower) || categoryLower.Contains(query))
            return true;

        // Common category mappings (Turkish)
        var categoryMappings = new Dictionary<string, List<string>>
        {
            { "süt ürünleri", new List<string> { "süt", "peynir", "yoğurt", "tereyağı", "ayran" } },
            { "et ürünleri", new List<string> { "et", "tavuk", "sosis", "sucuk", "salam" } },
            { "sebze", new List<string> { "domates", "salatalık", "marul", "soğan", "biber" } },
            { "meyve", new List<string> { "elma", "muz", "portakal", "üzüm", "çilek" } },
            { "içecek", new List<string> { "su", "kola", "meyve suyu", "çay", "kahve" } },
            { "unlu mamüller", new List<string> { "ekmek", "pasta", "börek", "poğaça", "simit" } },
            { "temel gıda", new List<string> { "pirinç", "bulgur", "makarna", "un", "şeker", "tuz", "yağ" } }
        };

        foreach (var mapping in categoryMappings)
        {
            if (categoryLower.Contains(mapping.Key) || mapping.Key.Contains(categoryLower))
            {
                // Check if query matches any item in that category
                if (mapping.Value.Any(item => query.Contains(item) || item.Contains(query)))
                    return true;
            }
        }

        return false;
    }
}
