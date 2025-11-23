using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using DTOs.DTOs.Requests;
using DTOs.DTOs.Responses;

namespace Domain.Services;

public class MarketService : IMarketService
{
    private readonly IMarketRepository _marketRepository;

    public MarketService(IMarketRepository marketRepository)
    {
        _marketRepository = marketRepository;
    }

    public async Task<IEnumerable<MarketResponseDTO>> GetAllMarketsAsync()
    {
        var markets = await _marketRepository.GetAllAsync();
        return markets.Select(MapToResponseDTO);
    }

    public async Task<MarketResponseDTO?> GetMarketByIdAsync(int id)
    {
        var market = await _marketRepository.GetByIdAsync(id);
        return market != null ? MapToResponseDTO(market) : null;
    }

    public async Task<IEnumerable<MarketResponseDTO>> SearchMarketsAsync(string searchTerm)
    {
        var markets = await _marketRepository.SearchByNameAsync(searchTerm);
        return markets.Select(MapToResponseDTO);
    }

    public async Task<MarketResponseDTO> CreateMarketAsync(CreateMarketDTO dto)
    {
        var market = new Market
        {
            MarketName = dto.MarketName,
            LogoUrl = dto.LogoUrl,
            Website = dto.Website
        };

        await _marketRepository.AddAsync(market);
        return MapToResponseDTO(market);
    }

    public async Task<MarketResponseDTO?> UpdateMarketAsync(int id, UpdateMarketDTO dto)
    {
        var market = await _marketRepository.GetByIdAsync(id);
        if (market == null)
        {
            return null;
        }

        // Update only provided fields
        if (!string.IsNullOrWhiteSpace(dto.MarketName))
        {
            market.MarketName = dto.MarketName;
        }

        if (dto.LogoUrl != null)
        {
            market.LogoUrl = dto.LogoUrl;
        }

        if (dto.Website != null)
        {
            market.Website = dto.Website;
        }

        await _marketRepository.UpdateAsync(market);
        return MapToResponseDTO(market);
    }

    public async Task<bool> DeleteMarketAsync(int id)
    {
        var market = await _marketRepository.GetByIdAsync(id);
        if (market == null)
        {
            return false;
        }

        await _marketRepository.DeleteAsync(id); // Soft delete
        return true;
    }

    private static MarketResponseDTO MapToResponseDTO(Market market)
    {
        return new MarketResponseDTO
        {
            Id = market.Id,
            MarketName = market.MarketName,
            LogoUrl = market.LogoUrl,
            Website = market.Website,
            CreatedAt = market.CreatedAt
        };
    }
}
