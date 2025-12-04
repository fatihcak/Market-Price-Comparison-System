using DTOs.DTOs.Responses;

namespace Domain.Interfaces.Services;

public interface ICityService
{
    Task<IEnumerable<CityResponseDTO>> GetAllCitiesAsync();
    Task<CityResponseDTO?> GetCityByIdAsync(int id);
}
