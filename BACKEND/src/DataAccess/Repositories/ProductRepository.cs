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
        return await _context.Products
            .Where(p => p.CategoryId == categoryId)
            .Include(p => p.Category)
            .OrderBy(p => p.ProductName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> SearchByNameAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await _context.Products
                .Include(p => p.Category)
                .OrderBy(p => p.ProductName)
                .ToListAsync();
        }

        return await _context.Products
            .Where(p => p.ProductName.Contains(searchTerm))
            .Include(p => p.Category)
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
        IQueryable<Product> query = null;
        foreach (var term in validTerms)
        {
            var lowerTerm = term.ToLower();
            var subQuery = _context.Products.Where(p => p.ProductName.ToLower().Contains(lowerTerm));
            query = query == null ? subQuery : query.Union(subQuery);
        }

        if (query == null) return new List<Product>();

        return await query
            .Include(p => p.Category)
            .OrderBy(p => p.ProductName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> SearchByBrandAsync(string brand)
    {
        if (string.IsNullOrWhiteSpace(brand))
        {
            return await GetAllAsync();
        }

        return await _context.Products
            .Where(p => p.Brand != null && p.Brand.Contains(brand))
            .Include(p => p.Category)
            .OrderBy(p => p.ProductName)
            .ToListAsync();
    }

    public async Task<Product?> GetProductWithCategoryAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Product>> GetAllWithDetailsAsync()
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.MarketProductPrices)
            .ThenInclude(mpp => mpp.Market)
            .OrderBy(p => p.ProductName)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductPriceHistory>> GetPriceHistoryByProductIdAsync(int productId)
    {
        return await _context.ProductPriceHistories
            .Include(h => h.MarketProductPrice)
            .Where(h => h.MarketProductPrice.ProductId == productId)
            .OrderBy(h => h.ChangedDate)
            .ToListAsync();
    }
}
