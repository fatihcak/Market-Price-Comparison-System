using Microsoft.AspNetCore.Mvc;
using Domain.Interfaces.Services;
using DTOs.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using API.Extensions;
using Microsoft.Extensions.Caching.Memory;
using DTOs.DTOs.Responses;

namespace API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("api/[controller]")]
public class MarketController : ControllerBase
{
    private readonly IMarketService _marketService;
    private readonly IMemoryCache _cache;

    public MarketController(IMarketService marketService, IMemoryCache cache)
    {
        _marketService = marketService;
        _cache = cache;
    }

    /// <summary>
    /// Get all markets (cached for 24 hours)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        // Check cache first
        if (_cache.TryGetValue(API.Constants.CacheKeys.AllMarkets, out IEnumerable<MarketResponseDTO>? cached))
        {
            return this.ApiOk(cached);
        }

        var markets = await _marketService.GetAllMarketsAsync();
        
        // Cache for 24 hours (markets rarely change)
        _cache.Set(API.Constants.CacheKeys.AllMarkets, markets, TimeSpan.FromHours(24));
        
        return this.ApiOk(markets);
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
            return this.ApiNotFound($"Market with ID {id} not found");
        }

        return this.ApiOk(market);
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
            return this.ApiBadRequest("Search term is required");
        }

        var markets = await _marketService.SearchMarketsAsync(term);
        return this.ApiOk(markets);
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
            return this.ApiBadRequest(ModelState);
        }

        var market = await _marketService.CreateMarketAsync(dto);
        return this.ApiCreated(nameof(GetById), new { id = market.Id }, market);
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
            return this.ApiBadRequest(ModelState);
        }

        var market = await _marketService.UpdateMarketAsync(id, dto);

        if (market == null)
        {
            return this.ApiNotFound($"Market with ID {id} not found");
        }

        return this.ApiOk(market);
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
            return this.ApiNotFound($"Market with ID {id} not found");
        }

        return NoContent();
    }
}

