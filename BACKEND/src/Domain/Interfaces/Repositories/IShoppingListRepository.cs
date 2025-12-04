using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IShoppingListRepository : IRepository<UserProductList>
{
    Task<IEnumerable<UserProductList>> GetBySessionIdAsync(string sessionId);
    Task<UserProductList?> GetBySessionAndProductIdAsync(string sessionId, int productId);
}
