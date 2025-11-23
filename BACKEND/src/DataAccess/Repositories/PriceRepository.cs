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
        return await _context.MarketProductPrices
            .Where(p => p.ProductId == productId)
            .Include(p => p.Market)
            .Include(p => p.District)
            .OrderBy(p => p.Price)
            .ToListAsync();
    }

    public async Task<IEnumerable<MarketProductPrice>> GetByMarketIdAsync(int marketId)
    {
        return await _context.MarketProductPrices
            .Where(p => p.MarketId == marketId)
            .Include(p => p.Product)
            .ToListAsync();
    }

    public async Task<IEnumerable<MarketProductPrice>> GetByDistrictIdAsync(int districtId)
    {
         return await _context.MarketProductPrices
            .Where(p => p.DistrictId == districtId)
            .Include(p => p.Product)
            .Include(p => p.Market)
            .ToListAsync();
    }
}
