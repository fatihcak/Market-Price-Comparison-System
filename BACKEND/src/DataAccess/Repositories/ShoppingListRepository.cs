using DataAccess.Data;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class ShoppingListRepository : Repository<UserProductList>, IShoppingListRepository
{
    public ShoppingListRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<UserProductList>> GetBySessionIdAsync(string sessionId)
    {
        return await _context.UserProductList
            .Where(l => l.SessionId == sessionId)
            .Include(l => l.Product)
            .ThenInclude(p => p.Category)
            .ToListAsync();
    }

    public async Task<UserProductList?> GetBySessionAndProductIdAsync(string sessionId, int productId)
    {
        return await _context.UserProductList
            .FirstOrDefaultAsync(l => l.SessionId == sessionId && l.ProductId == productId);
    }
}
