using DataAccess.Data;
using Domain.Entities;
using Domain.Interfaces.Repositories;

namespace DataAccess.Repositories;

public class CityRepository : Repository<City>, ICityRepository
{
    public CityRepository(AppDbContext context) : base(context)
    {
    }
}
