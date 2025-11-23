using Microsoft.EntityFrameworkCore;
using DataAccess.Data;
using Domain.Entities;
using Domain.Interfaces.Repositories;

namespace DataAccess.Repositories;

public class MarketRepository : Repository<Market>, IMarketRepository
{
    public MarketRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Market>> SearchByNameAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllAsync();
        }

        return await _context.Markets
            .Where(m => m.MarketName.Contains(searchTerm))
            .OrderBy(m => m.MarketName)
            .ToListAsync();
    }
}
