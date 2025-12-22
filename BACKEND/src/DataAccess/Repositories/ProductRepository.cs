using Microsoft.EntityFrameworkCore;
using DataAccess.Data;
using Domain.Entities;
using Domain.Interfaces.Repositories;

namespace DataAccess.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId)
    {
        return await _context.Product
            .AsNoTracking()
            .AsSplitQuery()
            .Where(p => p.CategoryId == categoryId)
            .Include(p => p.Category)
            .Include(p => p.MarketProductPrices)
            .ThenInclude(mpp => mpp.Market)
            .OrderBy(p => p.ProductName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> SearchByNameAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await _context.Product
                .Include(p => p.Category)
                .Include(p => p.MarketProductPrices)
                .ThenInclude(mpp => mpp.Market)
                .OrderBy(p => p.ProductName)
                .ToListAsync();
        }

        return await _context.Product
            .Where(p => p.ProductName.Contains(searchTerm))
            .Include(p => p.Category)
            .Include(p => p.MarketProductPrices)
            .ThenInclude(mpp => mpp.Market)
            .OrderBy(p => p.ProductName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> SearchByNamesAsync(IEnumerable<string> searchTerms)
    {
        if (searchTerms == null || !searchTerms.Any())
        {
            return new List<Product>();
        }

        // Filter out empty strings to avoid matching everything
        var validTerms = searchTerms.Where(t => !string.IsNullOrWhiteSpace(t)).ToList();
        
        if (!validTerms.Any())
        {
             return new List<Product>();
        }

        // EF Core might not translate validTerms.Any(term => p.ProductName.Contains(term)) correctly in all providers.
        // We use Union to build an OR query dynamically.
        IQueryable<Product>? query = null;
        foreach (var term in validTerms)
        {
            if (term == null) continue;
            var lowerTerm = term.ToLower();
            var subQuery = _context.Product.Where(p => p.ProductName.ToLower().Contains(lowerTerm));
            query = query == null ? subQuery : query.Union(subQuery);
        }

        if (query == null) return new List<Product>();

        return await query
            .Include(p => p.Category)
            .Include(p => p.MarketProductPrices)
            .ThenInclude(mpp => mpp.Market)
            .OrderBy(p => p.ProductName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> SearchByBrandAsync(string brand)
    {
        if (string.IsNullOrWhiteSpace(brand))
        {
            return await GetAllAsync();
        }

        return await _context.Product
            .Where(p => p.Brand != null && p.Brand.Contains(brand))
            .Include(p => p.Category)
            .Include(p => p.MarketProductPrices)
            .ThenInclude(mpp => mpp.Market)
            .OrderBy(p => p.ProductName)
            .ToListAsync();
    }

    public async Task<Product?> GetProductWithCategoryAsync(int id)
    {
        return await _context.Product
            .Include(p => p.Category)
            .Include(p => p.MarketProductPrices)
            .ThenInclude(mpp => mpp.Market)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Product>> GetAllWithDetailsAsync()
    {
        // AsNoTracking for read-only queries, AsSplitQuery to avoid Cartesian explosion
        return await _context.Product
            .AsNoTracking()
            .AsSplitQuery()
            .Include(p => p.Category)
            .Include(p => p.MarketProductPrices)
                .ThenInclude(mpp => mpp.Market)
            .OrderBy(p => p.ProductName)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductPriceHistory>> GetPriceHistoryByProductIdAsync(int productId)
    {
        return await _context.ProductPriceHistory
            .Include(h => h.MarketProductPrice)
            .Where(h => h.MarketProductPrice.ProductId == productId)
            .OrderBy(h => h.ChangedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> SearchWithFuzzyAsync(string searchTerm)
    {
        // 1. Fetch all products (or a reasonable subset if DB is huge)
        // For a small-to-medium catalog, fetching names is fine.
        var allProducts = await _context.Product.ToListAsync();

        // 2. Perform Fuzzy Matching in Memory
        var matches = allProducts
            .Select(p => new { Product = p, Distance = ComputeLevenshteinDistance(searchTerm.ToLower(), p.ProductName.ToLower()) })
            .Where(x => x.Distance <= 3) // Allow up to 3 edits
            .OrderBy(x => x.Distance)
            .Take(5)
            .Select(x => x.Product)
            .ToList();

        return matches;
    }

    public async Task<IEnumerable<Product>> SearchByCategoryAsync(string categoryName)
    {
        var term = categoryName.ToLower();
        return await _context.Product
            .Include(p => p.Category)
            .Include(p => p.MarketProductPrices)
            .ThenInclude(mpp => mpp.Market)
            .Where(p => p.Category.CategoryName.ToLower().Contains(term))
            .Take(5) // Limit results
            .ToListAsync();
    }

    private int ComputeLevenshteinDistance(string s, string t)
    {
        int n = s.Length;
        int m = t.Length;
        int[,] d = new int[n + 1, m + 1];

        if (n == 0) return m;
        if (m == 0) return n;

        for (int i = 0; i <= n; d[i, 0] = i++) { }
        for (int j = 0; j <= m; d[0, j] = j++) { }

        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= m; j++)
            {
                int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);
            }
        }
        return d[n, m];
    }
}
