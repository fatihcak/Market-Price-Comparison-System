using DataAccess.Data;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class PriceRepository : Repository<MarketProductPrice>, IPriceRepository
{
    public PriceRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<MarketProductPrice>> GetByProductIdAsync(int productId)
    {
        var prices = await _context.MarketProductPrice
            .Where(p => p.ProductId == productId)
            .Include(p => p.Market)
            .Include(p => p.District)
            .ToListAsync();

        return prices.OrderBy(p => p.Price);
    }

    public async Task<IEnumerable<MarketProductPrice>> GetByMarketIdAsync(int marketId)
    {
        return await _context.MarketProductPrice
            .Where(p => p.MarketId == marketId)
            .Include(p => p.Product)
            .ToListAsync();
    }

    public async Task<IEnumerable<MarketProductPrice>> GetByDistrictIdAsync(int districtId)
    {
         return await _context.MarketProductPrice
            .Where(p => p.DistrictId == districtId)
            .Include(p => p.Product)
            .Include(p => p.Market)
            .ToListAsync();
    }

    public async Task<IEnumerable<MarketProductPrice>> GetPricesForProductsAsync(IEnumerable<int> productIds)
    {
        return await _context.MarketProductPrice
            .Where(p => productIds.Contains(p.ProductId))
            .Include(p => p.Market)
            .ToListAsync();
    }

    public async Task AddPriceHistoryAsync(ProductPriceHistory history)
    {
        await _context.ProductPriceHistory.AddAsync(history);
        await _context.SaveChangesAsync();
    }
}
