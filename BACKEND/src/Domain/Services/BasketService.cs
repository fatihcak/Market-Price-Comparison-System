using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using DTOs.DTOs.Responses;

namespace Domain.Services;

public class BasketService : IBasketService
{
    private readonly IBasketRepository _basketRepository;
    private readonly IPriceRepository _priceRepository;
    private readonly IRepository<Market> _marketRepository;

    public BasketService(
        IBasketRepository basketRepository,
        IPriceRepository priceRepository,
        IRepository<Market> marketRepository)
    {
        _basketRepository = basketRepository;
        _priceRepository = priceRepository;
        _marketRepository = marketRepository;
    }

    public async Task AddToBasketAsync(string sessionId, int productId, int quantity)
    {
        var existingItem = await _basketRepository.GetBySessionAndProductIdAsync(sessionId, productId);
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
            if (existingItem.Quantity <= 0)
            {
                await _basketRepository.DeleteAsync(existingItem.Id);
            }
            else
            {
                await _basketRepository.UpdateAsync(existingItem);
            }
        }
        else
        {
            if (quantity > 0)
            {
                await _basketRepository.AddAsync(new UserProductList
                {
                    SessionId = sessionId,
                    ProductId = productId,
                    Quantity = quantity,
                    AddedDate = DateTime.UtcNow
                });
            }
        }
    }

    public async Task RemoveFromBasketAsync(string sessionId, int productId)
    {
        var item = await _basketRepository.GetBySessionAndProductIdAsync(sessionId, productId);
        if (item != null)
        {
            await _basketRepository.DeleteAsync(item.Id);
        }
    }

    public async Task ClearBasketAsync(string sessionId)
    {
        await _basketRepository.DeleteBySessionIdAsync(sessionId);
    }

    public async Task<IEnumerable<UserProductListDTO>> GetBasketAsync(string sessionId)
    {
        var items = await _basketRepository.GetBySessionIdAsync(sessionId);
        
        // Get average prices for display
        var productIds = items.Select(i => i.ProductId).Distinct();
        var prices = await _priceRepository.GetPricesForProductsAsync(productIds);
        
        return items.Select(item =>
        {
            var productPrices = prices.Where(p => p.ProductId == item.ProductId);
            var avgPrice = productPrices.Any() ? productPrices.Average(p => p.Price) : (decimal?)null;

            return new UserProductListDTO
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.Product.ProductName,
                ProductImage = item.Product.ImageUrl ?? item.Product.Category?.Icon,
                Quantity = item.Quantity,
                EstimatedPrice = avgPrice
            };
        });
    }

    public async Task<IEnumerable<BasketComparisonDTO>> CompareBasketAsync(string sessionId)
    {
        var basketItems = await _basketRepository.GetBySessionIdAsync(sessionId);
        if (!basketItems.Any()) return Enumerable.Empty<BasketComparisonDTO>();

        var productIds = basketItems.Select(i => i.ProductId).Distinct();
        var allPrices = await _priceRepository.GetPricesForProductsAsync(productIds);
        var markets = await _marketRepository.GetAllAsync();

        var result = new List<BasketComparisonDTO>();

        foreach (var market in markets)
        {
            decimal total = 0;
            var missingItems = new List<string>();
            int missingCount = 0;

            foreach (var item in basketItems)
            {
                var price = allPrices.FirstOrDefault(p => p.ProductId == item.ProductId && p.MarketId == market.Id);
                if (price != null)
                {
                    total += price.Price * item.Quantity;
                }
                else
                {
                    missingCount++;
                    missingItems.Add(item.Product.ProductName);
                }
            }

            result.Add(new BasketComparisonDTO
            {
                MarketName = market.MarketName,
                MarketLogoUrl = market.LogoUrl ?? "",
                TotalPrice = total,
                MissingItemCount = missingCount,
                MissingItems = missingItems
            });
        }

        // Mark cheapest (only among those with 0 missing items, or least missing items)
        // Logic: Prioritize completeness, then price.
        var completeBaskets = result.Where(r => r.MissingItemCount == 0).ToList();
        if (completeBaskets.Any())
        {
            var minPrice = completeBaskets.Min(r => r.TotalPrice);
            foreach (var r in result)
            {
                if (r.MissingItemCount == 0 && r.TotalPrice == minPrice)
                {
                    r.IsCheapest = true;
                }
            }
        }
        else
        {
            // If no complete basket, find the one with least missing items and lowest price
            var minMissing = result.Min(r => r.MissingItemCount);
            var candidates = result.Where(r => r.MissingItemCount == minMissing).ToList();
            var minPrice = candidates.Min(r => r.TotalPrice);
             foreach (var r in result)
            {
                if (r.MissingItemCount == minMissing && r.TotalPrice == minPrice)
                {
                    r.IsCheapest = true;
                }
            }
        }

        return result.OrderBy(r => r.MissingItemCount).ThenBy(r => r.TotalPrice);
    }
}
