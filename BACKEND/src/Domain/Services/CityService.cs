using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using DTOs.DTOs.Responses;

namespace Domain.Services;

public class CityService : ICityService
{
    private readonly ICityRepository _cityRepository;

    public CityService(ICityRepository cityRepository)
    {
        _cityRepository = cityRepository;
    }

    public async Task<IEnumerable<CityResponseDTO>> GetAllCitiesAsync()
    {
        var cities = await _cityRepository.GetAllAsync();
        return cities.Select(MapToResponseDTO);
    }

    public async Task<CityResponseDTO?> GetCityByIdAsync(int id)
    {
        var city = await _cityRepository.GetByIdAsync(id);
        return city != null ? MapToResponseDTO(city) : null;
    }

    private static CityResponseDTO MapToResponseDTO(City city)
    {
        return new CityResponseDTO
        {
            Id = city.Id,
            CityName = city.CityName
        };
    }
}
