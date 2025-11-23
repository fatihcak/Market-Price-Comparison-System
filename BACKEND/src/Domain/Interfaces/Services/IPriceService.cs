using DTOs.DTOs.Requests;
using DTOs.DTOs.Responses;

namespace Domain.Interfaces.Services;

public interface IPriceService
{
    Task<IEnumerable<PriceResponseDTO>> GetPricesByProductIdAsync(int productId);
    Task<IEnumerable<PriceResponseDTO>> GetPricesByMarketIdAsync(int marketId);
    Task<IEnumerable<PriceResponseDTO>> GetPricesByDistrictIdAsync(int districtId);
    Task<PriceResponseDTO> AddPriceAsync(CreatePriceDTO dto);
    Task<PriceResponseDTO?> UpdatePriceAsync(int id, UpdatePriceDTO dto);
    Task<bool> DeletePriceAsync(int id);
}
