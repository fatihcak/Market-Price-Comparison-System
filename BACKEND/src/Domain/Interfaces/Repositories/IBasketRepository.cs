using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IBasketRepository : IRepository<UserProductList>
{
    Task<IEnumerable<UserProductList>> GetBySessionIdAsync(string sessionId);
    Task<UserProductList?> GetBySessionAndProductIdAsync(string sessionId, int productId);
    Task DeleteBySessionIdAsync(string sessionId);
}
