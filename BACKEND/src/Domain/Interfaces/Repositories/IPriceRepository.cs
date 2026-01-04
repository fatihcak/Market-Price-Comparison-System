using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IPriceRepository : IRepository<MarketProductPrice>
{
    Task<IEnumerable<MarketProductPrice>> GetByProductIdAsync(int productId);
    Task<IEnumerable<MarketProductPrice>> GetByMarketIdAsync(int marketId);
    Task<IEnumerable<MarketProductPrice>> GetByDistrictIdAsync(int districtId);
    Task<IEnumerable<MarketProductPrice>> GetPricesForProductsAsync(IEnumerable<int> productIds);
    Task AddPriceHistoryAsync(ProductPriceHistory history);
    
    // Pagination methods
    Task<(IEnumerable<MarketProductPrice> Items, int TotalCount)> GetByProductIdWithPaginationAsync(int productId, int page, int pageSize);
    Task<(IEnumerable<MarketProductPrice> Items, int TotalCount)> GetByMarketIdWithPaginationAsync(int marketId, int page, int pageSize);
    Task<(IEnumerable<MarketProductPrice> Items, int TotalCount)> GetByDistrictIdWithPaginationAsync(int districtId, int page, int pageSize);
}
