using Microsoft.AspNetCore.Mvc;
using Domain.Interfaces.Services;
using Asp.Versioning;
using API.Extensions;

namespace API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("api/[controller]")]
public class DistrictController : ControllerBase
{
    private readonly IDistrictService _districtService;

    public DistrictController(IDistrictService districtService)
    {
        _districtService = districtService;
    }

    [HttpGet("city/{cityId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCity(int cityId)
    {
        var districts = await _districtService.GetDistrictsByCityIdAsync(cityId);
        return this.ApiOk(districts);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var district = await _districtService.GetDistrictByIdAsync(id);
        if (district == null) return this.ApiNotFound($"District with ID {id} not found");
        return this.ApiOk(district);
    }
}

