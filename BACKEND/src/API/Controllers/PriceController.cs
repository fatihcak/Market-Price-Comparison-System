using Microsoft.AspNetCore.Mvc;
using Domain.Interfaces.Services;
using DTOs.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;

namespace API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("api/[controller]")]
public class PriceController : ControllerBase
{
    private readonly IPriceService _priceService;

    public PriceController(IPriceService priceService)
    {
        _priceService = priceService;
    }

    [HttpGet("product/{productId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByProduct(int productId)
    {
        var prices = await _priceService.GetPricesByProductIdAsync(productId);
        return Ok(prices);
    }

    [HttpGet("market/{marketId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByMarket(int marketId)
    {
        var prices = await _priceService.GetPricesByMarketIdAsync(marketId);
        return Ok(prices);
    }

    [HttpGet("district/{districtId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByDistrict(int districtId)
    {
        var prices = await _priceService.GetPricesByDistrictIdAsync(districtId);
        return Ok(prices);
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddPrice([FromBody] CreatePriceDTO dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var price = await _priceService.AddPriceAsync(dto);
        // Assuming we don't have a direct GetById endpoint for price exposed or needed for CreatedAtAction easily without ID
        // But we do return the created object.
        return StatusCode(StatusCodes.Status201Created, price);
    }

    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePrice(int id, [FromBody] UpdatePriceDTO dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var price = await _priceService.UpdatePriceAsync(id, dto);
        if (price == null) return NotFound(new { message = $"Price with ID {id} not found" });
        return Ok(price);
    }

    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePrice(int id)
    {
        var result = await _priceService.DeletePriceAsync(id);
        if (!result) return NotFound(new { message = $"Price with ID {id} not found" });
        return NoContent();
    }
}
