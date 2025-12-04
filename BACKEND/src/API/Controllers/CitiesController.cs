using Microsoft.AspNetCore.Mvc;
using Domain.Interfaces.Services;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CitiesController : ControllerBase
{
    private readonly ICityService _cityService;

    public CitiesController(ICityService cityService)
    {
        _cityService = cityService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var cities = await _cityService.GetAllCitiesAsync();
        return Ok(cities);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var city = await _cityService.GetCityByIdAsync(id);
        if (city == null) return NotFound(new { message = $"City with ID {id} not found" });
        return Ok(city);
    }
}
