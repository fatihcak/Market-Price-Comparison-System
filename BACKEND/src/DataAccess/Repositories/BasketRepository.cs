using DataAccess.Data;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class BasketRepository : Repository<UserProductList>, IBasketRepository
{
    public BasketRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<UserProductList>> GetBySessionIdAsync(string sessionId)
    {
        return await _context.UserProductList
            .Include(x => x.Product)
            .ThenInclude(p => p.Category)
            .Where(x => x.SessionId == sessionId)
            .ToListAsync();
    }

    public async Task<UserProductList?> GetBySessionAndProductIdAsync(string sessionId, int productId)
    {
        return await _context.UserProductList
            .FirstOrDefaultAsync(x => x.SessionId == sessionId && x.ProductId == productId);
    }

    public async Task DeleteBySessionIdAsync(string sessionId)
    {
        var items = await _context.UserProductList
            .Where(x => x.SessionId == sessionId)
            .ToListAsync();
            
        _context.UserProductList.RemoveRange(items);
        await _context.SaveChangesAsync();
    }
}
