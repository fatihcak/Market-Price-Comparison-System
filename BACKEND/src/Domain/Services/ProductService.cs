using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using DTOs.DTOs.Requests;
using DTOs.DTOs.Responses;

namespace Domain.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<ProductResponseDTO>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAllWithDetailsAsync();
        return products.Select(p => MapToResponseDTO(p));
    }

    public async Task<(IEnumerable<ProductResponseDTO> Products, int TotalCount)> GetProductsWithPaginationAsync(int page, int pageSize)
    {
        var (products, totalCount) = await _productRepository.SearchByNameWithPaginationAsync("", page, pageSize);
        var dtos = products.Select(p => MapToResponseDTO(p));
        return (dtos, totalCount);
    }


    public async Task<ProductResponseDTO?> GetProductByIdAsync(int id)
    {
        var product = await _productRepository.GetProductWithCategoryAsync(id);
        return product != null ? MapToResponseDTO(product) : null;
    }

    public async Task<IEnumerable<ProductResponseDTO>> GetProductsByCategoryAsync(int categoryId)
    {
        var products = await _productRepository.GetByCategoryIdAsync(categoryId);
        return products.Select(p => MapToResponseDTO(p));
    }

    public async Task<IEnumerable<ProductResponseDTO>> SearchProductsAsync(string searchTerm)
    {
        var products = await _productRepository.SearchByNameAsync(searchTerm);
        return products.Select(p => MapToResponseDTO(p));
    }

    public async Task<IEnumerable<ProductResponseDTO>> SearchByBrandAsync(string brand)
    {
        var products = await _productRepository.SearchByBrandAsync(brand);
        return products.Select(MapToResponseDTO);
    }

    public async Task<ProductResponseDTO> CreateProductAsync(CreateProductDTO dto)
    {
        var product = new Product
        {
            CategoryId = dto.CategoryId,
            ProductName = dto.ProductName,
            Brand = dto.Brand,
            Unit = dto.Unit,
            LastUpdated = DateTime.UtcNow
        };

        await _productRepository.AddAsync(product);

        // Get product with category for response
        var createdProduct = await _productRepository.GetProductWithCategoryAsync(product.Id);
        return MapToResponseDTO(createdProduct!);
    }

    public async Task<ProductResponseDTO?> UpdateProductAsync(int id, UpdateProductDTO dto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return null;
        }

        // Update only provided fields
        if (!string.IsNullOrWhiteSpace(dto.ProductName))
        {
            product.ProductName = dto.ProductName;
        }

        if (dto.Brand != null)
        {
            product.Brand = dto.Brand;
        }

        if (!string.IsNullOrWhiteSpace(dto.Unit))
        {
            product.Unit = dto.Unit;
        }

        product.LastUpdated = DateTime.UtcNow;

        await _productRepository.UpdateAsync(product);

        // Get product with category for response
        var updatedProduct = await _productRepository.GetProductWithCategoryAsync(id);
        return MapToResponseDTO(updatedProduct!);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return false;
        }

        await _productRepository.DeleteAsync(id); // Soft delete
        return true;
    }

    public async Task<IEnumerable<ProductPriceHistoryDTO>> GetProductPriceHistoryAsync(int productId, int days)
    {
        // FIXED: Removed N+1 query - using the already loaded product instead of fetching all products
        var product = await _productRepository.GetProductWithCategoryAsync(productId);
        if (product == null) return Enumerable.Empty<ProductPriceHistoryDTO>();

        var history = await _productRepository.GetPriceHistoryByProductIdAsync(productId);

        // OPTIMIZATION: No longer loading all products - using the already fetched 'product'
        var result = new List<ProductPriceHistoryDTO>();
        var startDate = DateTime.UtcNow.Date.AddDays(-days);
        var endDate = DateTime.UtcNow.Date;

        var historyLookup = history.GroupBy(h => h.MarketProductPriceId).ToDictionary(g => g.Key, g => g.ToList());

        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            var dailyPrices = new List<decimal>();

            foreach (var mpp in product.MarketProductPrices)
            {
                decimal priceOnDate = mpp.Price;
                var mppLastUpdated = mpp.LastUpdated ?? DateTime.MinValue;

                if (mppLastUpdated.Date > date)
                {
                    if (historyLookup.TryGetValue(mpp.Id, out var mppHistory))
                    {
                        var relevantHistory = mppHistory
                            .Where(h => h.ChangedDate.Date > date)
                            .OrderBy(h => h.ChangedDate)
                            .FirstOrDefault();

                        if (relevantHistory != null)
                        {
                            priceOnDate = relevantHistory.Price;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                         continue;
                    }
                }

                dailyPrices.Add(priceOnDate);
            }

            if (dailyPrices.Any())
            {
                result.Add(new ProductPriceHistoryDTO
                {
                    Date = date,
                    MinPrice = dailyPrices.Min(),
                    MaxPrice = dailyPrices.Max(),
                    AveragePrice = Math.Round(dailyPrices.Average(), 2)
                });
            }
        }

        return result;
    }

    public async Task<(IEnumerable<ProductResponseDTO> Products, int TotalCount)> GetProductsOrderedByDiscountAsync(int page, int pageSize)
    {
        // Get all products with details to calculate discount
        var products = await _productRepository.GetAllWithDetailsAsync();
        
        // Map to DTO and filter to only products with discount > 0
        var productsWithDiscount = products
            .Select(p => MapToResponseDTO(p))
            .Where(p => p.Discount > 0)
            .OrderByDescending(p => p.Discount)
            .ToList();
        
        var totalCount = productsWithDiscount.Count;
        var pagedProducts = productsWithDiscount
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
        
        return (pagedProducts, totalCount);
    }

    private static ProductResponseDTO MapToResponseDTO(Product product)
    {
        var prices = product.MarketProductPrices;
        var minPrice = prices.Any() ? prices.Min(p => p.Price) : 0;
        var maxPrice = prices.Any() ? prices.Max(p => p.Price) : 0;
        var cheapestMarket = prices.OrderBy(p => p.Price).FirstOrDefault()?.Market?.MarketName ?? "Unknown";
        
        // Count unique markets
        var marketCount = prices.Select(p => p.MarketId).Distinct().Count();
        
        var discount = maxPrice > minPrice ? (int)((maxPrice - minPrice) / maxPrice * 100) : 0;

        return new ProductResponseDTO
        {
            Id = product.Id,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.CategoryName ?? string.Empty,
            ProductName = product.ProductName,
            Brand = product.Brand,
            Unit = product.Unit ?? string.Empty,
            Price = minPrice,
            OldPrice = maxPrice > minPrice ? maxPrice : null,
            Discount = discount,
            MarketName = cheapestMarket,
            MarketCount = marketCount,
            LastUpdated = product.LastUpdated,
            CreatedAt = null, // BaseEntity.CreatedAt is ignored in AppDbContext
            ImageUrl = !string.IsNullOrWhiteSpace(product.ImageUrl) ? product.ImageUrl : product.Category?.Icon
        };
    }
}
