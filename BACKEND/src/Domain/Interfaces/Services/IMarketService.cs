using DTOs.DTOs.Requests;
using DTOs.DTOs.Responses;

namespace Domain.Interfaces.Services;

public interface IMarketService
{
    Task<IEnumerable<MarketResponseDTO>> GetAllMarketsAsync();
    Task<MarketResponseDTO?> GetMarketByIdAsync(int id);
    Task<IEnumerable<MarketResponseDTO>> SearchMarketsAsync(string searchTerm);
    Task<MarketResponseDTO> CreateMarketAsync(CreateMarketDTO dto);
    Task<MarketResponseDTO?> UpdateMarketAsync(int id, UpdateMarketDTO dto);
    Task<bool> DeleteMarketAsync(int id);
}
