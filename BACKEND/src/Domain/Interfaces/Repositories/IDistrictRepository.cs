using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IDistrictRepository : IRepository<District>
{
    Task<IEnumerable<District>> GetByCityIdAsync(int cityId);
}
