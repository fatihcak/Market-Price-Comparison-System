using Microsoft.EntityFrameworkCore;
using DataAccess.Data;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Utilities;
using Domain.Constants;

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
        // OPTIMIZATION: Added limit to prevent loading all products
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await _context.Product
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.MarketProductPrices)
                .ThenInclude(mpp => mpp.Market)
                .OrderBy(p => p.ProductName)
                .Take(50) // Limit to 50 products when no search term
                .ToListAsync();
        }

        return await _context.Product
            .AsNoTracking()
            .Where(p => p.ProductName.Contains(searchTerm))
            .Include(p => p.Category)
            .Include(p => p.MarketProductPrices)
            .ThenInclude(mpp => mpp.Market)
            .OrderBy(p => p.ProductName)
            .Take(50) // Limit to 50 results
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
        // OPTIMIZATION: Added limit to prevent loading all products
        if (string.IsNullOrWhiteSpace(brand))
        {
            return await _context.Product
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.MarketProductPrices)
                .ThenInclude(mpp => mpp.Market)
                .OrderBy(p => p.ProductName)
                .Take(50)
                .ToListAsync();
        }

        return await _context.Product
            .AsNoTracking()
            .Where(p => p.Brand != null && p.Brand.Contains(brand))
            .Include(p => p.Category)
            .Include(p => p.MarketProductPrices)
            .ThenInclude(mpp => mpp.Market)
            .OrderBy(p => p.ProductName)
            .Take(50) // Limit to 50 results
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
        if (string.IsNullOrWhiteSpace(searchTerm))
            return new List<Product>();

        searchTerm = searchTerm.Trim().ToLower();

        // OPTIMIZATION 1: Try exact/contains match first (database-level, very fast)
        var exactMatches = await _context.Product
            .AsNoTracking()
            .Where(p => p.ProductName.ToLower().Contains(searchTerm))
            .Include(p => p.Category)
            .Include(p => p.MarketProductPrices)
                .ThenInclude(mpp => mpp.Market)
            .OrderBy(p => p.ProductName)
            .Take(10)
            .ToListAsync();

        if (exactMatches.Any())
            return exactMatches;

        // OPTIMIZATION 2: Only fetch IDs and names for fuzzy matching (much lighter than full products)
        var productNamesOnly = await _context.Product
            .AsNoTracking()
            .Select(p => new { p.Id, p.ProductName })
            .ToListAsync();

        // OPTIMIZATION 3: Perform fuzzy matching in memory on lightweight data
        var fuzzyMatchIds = productNamesOnly
            .Select(p => new {
                p.Id,
                Distance = StringUtilities.ComputeLevenshteinDistance(searchTerm, p.ProductName.ToLower())
            })
            .Where(x => x.Distance <= 3) // Allow up to 3 character edits
            .OrderBy(x => x.Distance)
            .Take(10)
            .Select(x => x.Id)
            .ToList();

        if (!fuzzyMatchIds.Any())
            return new List<Product>();

        // OPTIMIZATION 4: Only fetch full details for matched products
        var matchedProducts = await _context.Product
            .AsNoTracking()
            .Where(p => fuzzyMatchIds.Contains(p.Id))
            .Include(p => p.Category)
            .Include(p => p.MarketProductPrices)
                .ThenInclude(mpp => mpp.Market)
            .ToListAsync();

        // Preserve distance ordering
        var orderedResults = fuzzyMatchIds
            .Select(id => matchedProducts.FirstOrDefault(p => p.Id == id))
            .Where(p => p != null)
            .ToList();

        return orderedResults!;
    }

    public async Task<IEnumerable<Product>> SearchByCategoryAsync(string categoryName)
    {
        var term = categoryName.ToLower();
        return await _context.Product
            .Include(p => p.Category)
            .Include(p => p.MarketProductPrices)
            .ThenInclude(mpp => mpp.Market)
            .Where(p => p.Category != null && p.Category.CategoryName.ToLower().Contains(term))
            .Take(5) // Limit results
            .ToListAsync();
    }

    // PAGINATION METHODS - For better performance and user experience
    public async Task<(IEnumerable<Product> Items, int TotalCount)> SearchByNameWithPaginationAsync(
        string searchTerm,
        int pageNumber,
        int pageSize)
    {
        // Ensure valid pagination parameters
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < AppConstants.Pagination.MinPageSize ? AppConstants.Pagination.DefaultPageSize : (pageSize > AppConstants.Pagination.MaxPageSize ? AppConstants.Pagination.MaxPageSize : pageSize);

        var query = string.IsNullOrWhiteSpace(searchTerm)
            ? _context.Product.AsNoTracking()
            : _context.Product.AsNoTracking().Where(p => p.ProductName.Contains(searchTerm));

        var totalCount = await query.CountAsync();

        var items = await query
            .Include(p => p.Category)
            .Include(p => p.MarketProductPrices)
                .ThenInclude(mpp => mpp.Market)
            .OrderBy(p => p.ProductName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<(IEnumerable<Product> Items, int TotalCount)> GetByCategoryIdWithPaginationAsync(
        int categoryId,
        int pageNumber,
        int pageSize)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < AppConstants.Pagination.MinPageSize ? AppConstants.Pagination.DefaultPageSize : (pageSize > AppConstants.Pagination.MaxPageSize ? AppConstants.Pagination.MaxPageSize : pageSize);

        var query = _context.Product
            .AsNoTracking()
            .Where(p => p.CategoryId == categoryId);

        var totalCount = await query.CountAsync();

        var items = await query
            .Include(p => p.Category)
            .Include(p => p.MarketProductPrices)
                .ThenInclude(mpp => mpp.Market)
            .OrderBy(p => p.ProductName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<(IEnumerable<Product> Items, int TotalCount)> SearchByBrandWithPaginationAsync(
        string brand,
        int pageNumber,
        int pageSize)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < AppConstants.Pagination.MinPageSize ? AppConstants.Pagination.DefaultPageSize : (pageSize > AppConstants.Pagination.MaxPageSize ? AppConstants.Pagination.MaxPageSize : pageSize);

        var query = string.IsNullOrWhiteSpace(brand)
            ? _context.Product.AsNoTracking()
            : _context.Product.AsNoTracking().Where(p => p.Brand != null && p.Brand.Contains(brand));

        var totalCount = await query.CountAsync();

        var items = await query
            .Include(p => p.Category)
            .Include(p => p.MarketProductPrices)
                .ThenInclude(mpp => mpp.Market)
            .OrderBy(p => p.ProductName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    /// <summary>
    /// OPTIMIZED: Calculate discount in SQL, filter and paginate at database level
    /// Avoids loading 3000+ products into memory
    /// </summary>
    public async Task<(IEnumerable<Product> Items, int TotalCount)> GetProductsOrderedByDiscountAsync(int page, int pageSize)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 20 : (pageSize > 100 ? 100 : pageSize);

        // Step 1: Query products with aggregated prices using projection
        // This calculates MIN/MAX at SQL level and filters products with actual discounts
        var discountedProductIds = await _context.Product
            .AsNoTracking()
            .Where(p => p.MarketProductPrices.Any())
            .Select(p => new
            {
                ProductId = p.Id,
                MinPrice = p.MarketProductPrices.Min(mp => mp.Price),
                MaxPrice = p.MarketProductPrices.Max(mp => mp.Price)
            })
            .Where(x => x.MaxPrice > x.MinPrice && x.MaxPrice > 0)
            .Select(x => new
            {
                x.ProductId,
                Discount = (double)((x.MaxPrice - x.MinPrice) / x.MaxPrice * 100)
            })
            .OrderByDescending(x => x.Discount)
            .ToListAsync();

        var totalCount = discountedProductIds.Count;

        // Step 2: Get paginated IDs
        var pagedIds = discountedProductIds
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => x.ProductId)
            .ToList();

        if (!pagedIds.Any())
            return (new List<Product>(), totalCount);

        // Step 3: Fetch full product details only for the paginated set
        var products = await _context.Product
            .AsNoTracking()
            .Where(p => pagedIds.Contains(p.Id))
            .Include(p => p.Category)
            .Include(p => p.MarketProductPrices)
                .ThenInclude(mpp => mpp.Market)
            .ToListAsync();

        // Step 4: Preserve discount ordering
        var orderedProducts = pagedIds
            .Select(id => products.FirstOrDefault(p => p.Id == id))
            .Where(p => p != null)
            .ToList();

        return (orderedProducts!, totalCount);
    }
}
