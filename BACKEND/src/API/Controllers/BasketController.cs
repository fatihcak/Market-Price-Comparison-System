using Domain.Interfaces.Services;
using DTOs.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BasketController : ControllerBase
{
    private readonly IBasketService _basketService;

    public BasketController(IBasketService basketService)
    {
        _basketService = basketService;
    }

    [HttpGet("{sessionId}")]
    public async Task<IActionResult> GetBasket(string sessionId)
    {
        var basket = await _basketService.GetBasketAsync(sessionId);
        return Ok(basket);
    }

    [HttpPost("{sessionId}")]
    public async Task<IActionResult> AddToBasket(string sessionId, [FromBody] AddToBasketDTO dto)
    {
        await _basketService.AddToBasketAsync(sessionId, dto.ProductId, dto.Quantity);
        return Ok();
    }

    [HttpDelete("{sessionId}/{productId}")]
    public async Task<IActionResult> RemoveFromBasket(string sessionId, int productId)
    {
        await _basketService.RemoveFromBasketAsync(sessionId, productId);
        return Ok();
    }

    [HttpDelete("{sessionId}")]
    public async Task<IActionResult> ClearBasket(string sessionId)
    {
        await _basketService.ClearBasketAsync(sessionId);
        return Ok();
    }

    [HttpGet("{sessionId}/compare")]
    public async Task<IActionResult> CompareBasket(string sessionId)
    {
        var comparison = await _basketService.CompareBasketAsync(sessionId);
        return Ok(comparison);
    }
}

public class AddToBasketDTO
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
