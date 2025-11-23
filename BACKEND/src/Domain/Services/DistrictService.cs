using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using DTOs.DTOs.Responses;

namespace Domain.Services;

public class DistrictService : IDistrictService
{
    private readonly IDistrictRepository _districtRepository;

    public DistrictService(IDistrictRepository districtRepository)
    {
        _districtRepository = districtRepository;
    }

    public async Task<IEnumerable<DistrictResponseDTO>> GetDistrictsByCityIdAsync(int cityId)
    {
        var districts = await _districtRepository.GetByCityIdAsync(cityId);
        return districts.Select(MapToResponseDTO);
    }

    public async Task<DistrictResponseDTO?> GetDistrictByIdAsync(int id)
    {
        var district = await _districtRepository.GetByIdAsync(id);
        return district != null ? MapToResponseDTO(district) : null;
    }

    private static DistrictResponseDTO MapToResponseDTO(District district)
    {
        return new DistrictResponseDTO
        {
            Id = district.Id,
            CityId = district.CityId,
            DistrictName = district.DistrictName
        };
    }
}
