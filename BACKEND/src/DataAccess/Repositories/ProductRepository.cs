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
}
