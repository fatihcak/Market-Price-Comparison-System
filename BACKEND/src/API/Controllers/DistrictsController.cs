using Microsoft.AspNetCore.Mvc;
using Domain.Interfaces.Services;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DistrictsController : ControllerBase
{
    private readonly IDistrictService _districtService;

    public DistrictsController(IDistrictService districtService)
    {
        _districtService = districtService;
    }

    [HttpGet("city/{cityId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCity(int cityId)
    {
        var districts = await _districtService.GetDistrictsByCityIdAsync(cityId);
        return Ok(districts);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var district = await _districtService.GetDistrictByIdAsync(id);
        if (district == null) return NotFound(new { message = $"District with ID {id} not found" });
        return Ok(district);
    }
}
