using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using DTOs.DTOs.Requests;
using DTOs.DTOs.Responses;

namespace Domain.Services;

public class PriceService : IPriceService
{
    private readonly IPriceRepository _priceRepository;

    public PriceService(IPriceRepository priceRepository)
    {
        _priceRepository = priceRepository;
    }

    public async Task<IEnumerable<PriceResponseDTO>> GetPricesByProductIdAsync(int productId)
    {
        var prices = await _priceRepository.GetByProductIdAsync(productId);
        return prices.Select(MapToResponseDTO);
    }

    public async Task<IEnumerable<PriceResponseDTO>> GetPricesByMarketIdAsync(int marketId)
    {
        var prices = await _priceRepository.GetByMarketIdAsync(marketId);
        return prices.Select(MapToResponseDTO);
    }

    public async Task<IEnumerable<PriceResponseDTO>> GetPricesByDistrictIdAsync(int districtId)
    {
        var prices = await _priceRepository.GetByDistrictIdAsync(districtId);
        return prices.Select(MapToResponseDTO);
    }

    public async Task<PriceResponseDTO> AddPriceAsync(CreatePriceDTO dto)
    {
        var price = new MarketProductPrice
        {
            MarketId = dto.MarketId,
            ProductId = dto.ProductId,
            DistrictId = dto.DistrictId,
            Price = dto.Price,
            LastUpdated = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        await _priceRepository.AddAsync(price);
        
        // Fetch full entity to include relations if needed, or just return basic info
        // For now returning basic info + IDs
        return MapToResponseDTO(price);
    }

    public async Task<PriceResponseDTO?> UpdatePriceAsync(int id, UpdatePriceDTO dto)
    {
        var price = await _priceRepository.GetByIdAsync(id);
        if (price == null) return null;

        price.Price = dto.Price;
        price.LastUpdated = DateTime.UtcNow;

        await _priceRepository.UpdateAsync(price);

        return MapToResponseDTO(price);
    }

    public async Task<bool> DeletePriceAsync(int id)
    {
        var price = await _priceRepository.GetByIdAsync(id);
        if (price == null) return false;

        await _priceRepository.DeleteAsync(id);
        return true;
    }

    private static PriceResponseDTO MapToResponseDTO(MarketProductPrice price)
    {
        return new PriceResponseDTO
        {
            Id = price.Id,
            MarketId = price.MarketId,
            MarketName = price.Market?.MarketName ?? string.Empty,
            ProductId = price.ProductId,
            ProductName = price.Product?.ProductName ?? string.Empty,
            DistrictId = price.DistrictId,
            DistrictName = price.District?.DistrictName ?? string.Empty,
            Price = price.Price,
            LastUpdated = price.LastUpdated
        };
    }
}
