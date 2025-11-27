using Microsoft.AspNetCore.Mvc;
using Domain.Interfaces.Services;
using DTOs.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MarketsController : ControllerBase
{
    private readonly IMarketService _marketService;

    public MarketsController(IMarketService marketService)
    {
        _marketService = marketService;
    }

    /// <summary>
    /// Get all markets
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var markets = await _marketService.GetAllMarketsAsync();
        return Ok(markets);
    }

    /// <summary>
    /// Get market by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var market = await _marketService.GetMarketByIdAsync(id);

        if (market == null)
        {
            return NotFound(new { message = $"Market with ID {id} not found" });
        }

        return Ok(market);
    }

    /// <summary>
    /// Search markets by name
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] string term)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return BadRequest(new { message = "Search term is required" });
        }

        var markets = await _marketService.SearchMarketsAsync(term);
        return Ok(markets);
    }

    /// <summary>
    /// Create new market
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateMarket([FromBody] CreateMarketDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var market = await _marketService.CreateMarketAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = market.Id }, market);
    }

    /// <summary>
    /// Update existing market
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMarket(int id, [FromBody] UpdateMarketDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var market = await _marketService.UpdateMarketAsync(id, dto);

        if (market == null)
        {
            return NotFound(new { message = $"Market with ID {id} not found" });
        }

        return Ok(market);
    }

    /// <summary>
    /// Delete market (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMarket(int id)
    {
        var result = await _marketService.DeleteMarketAsync(id);

        if (!result)
        {
            return NotFound(new { message = $"Market with ID {id} not found" });
        }

        return NoContent();
    }
}
