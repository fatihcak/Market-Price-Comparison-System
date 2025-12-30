using Microsoft.AspNetCore.Mvc;
using Domain.Interfaces.Services;
using Asp.Versioning;
using API.Extensions;

namespace API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("api/[controller]")]
public class CityController : ControllerBase
{
    private readonly ICityService _cityService;

    public CityController(ICityService cityService)
    {
        _cityService = cityService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var cities = await _cityService.GetAllCitiesAsync();
        return this.ApiOk(cities);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var city = await _cityService.GetCityByIdAsync(id);
        if (city == null) return this.ApiNotFound($"City with ID {id} not found");
        return this.ApiOk(city);
    }
}

