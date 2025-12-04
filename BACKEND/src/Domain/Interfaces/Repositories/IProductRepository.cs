using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId);
    Task<IEnumerable<Product>> SearchByNameAsync(string searchTerm);
    Task<IEnumerable<Product>> SearchByBrandAsync(string brand);
    Task<Product?> GetProductWithCategoryAsync(int id);
    Task<IEnumerable<Product>> GetAllWithDetailsAsync();
    Task<IEnumerable<ProductPriceHistory>> GetPriceHistoryByProductIdAsync(int productId);
}
