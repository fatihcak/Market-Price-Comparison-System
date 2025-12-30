using Domain.Interfaces.Services;
using DTOs.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using API.Extensions;

namespace API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
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
        return this.ApiOk(basket);
    }

    [HttpPost("{sessionId}")]
    public async Task<IActionResult> AddToBasket(string sessionId, [FromBody] AddToBasketDTO dto)
    {
        await _basketService.AddToBasketAsync(sessionId, dto.ProductId, dto.Quantity);
        return this.ApiOk("Item added to basket");
    }

    [HttpDelete("{sessionId}/{productId}")]
    public async Task<IActionResult> RemoveFromBasket(string sessionId, int productId)
    {
        await _basketService.RemoveFromBasketAsync(sessionId, productId);
        return this.ApiOk("Item removed from basket");
    }

    [HttpDelete("{sessionId}")]
    public async Task<IActionResult> ClearBasket(string sessionId)
    {
        await _basketService.ClearBasketAsync(sessionId);
        return this.ApiOk("Basket cleared");
    }

    [HttpGet("{sessionId}/compare")]
    public async Task<IActionResult> CompareBasket(string sessionId)
    {
        var comparison = await _basketService.CompareBasketAsync(sessionId);
        return this.ApiOk(comparison);
    }
}

public class AddToBasketDTO
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

