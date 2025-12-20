using DataAccess.Data;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class DistrictRepository : Repository<District>, IDistrictRepository
{
    public DistrictRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<District>> GetByCityIdAsync(int cityId)
    {
        return await _context.District
            .Where(d => d.CityId == cityId)
            .OrderBy(d => d.DistrictName)
            .ToListAsync();
    }
}
