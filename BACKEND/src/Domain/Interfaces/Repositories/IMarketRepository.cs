using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IMarketRepository : IRepository<Market>
{
    Task<IEnumerable<Market>> SearchByNameAsync(string searchTerm);
}
