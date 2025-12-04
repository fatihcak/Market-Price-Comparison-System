using DataAccess.Data;
using Domain.Entities;
using Domain.Interfaces.Repositories;

namespace DataAccess.Repositories;

public class CategoryRepository : Repository<ProductCategory>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context)
    {
    }
}
