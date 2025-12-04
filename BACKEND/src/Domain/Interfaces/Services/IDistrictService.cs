using DTOs.DTOs.Responses;

namespace Domain.Interfaces.Services;

public interface IDistrictService
{
    Task<IEnumerable<DistrictResponseDTO>> GetDistrictsByCityIdAsync(int cityId);
    Task<DistrictResponseDTO?> GetDistrictByIdAsync(int id);
}
