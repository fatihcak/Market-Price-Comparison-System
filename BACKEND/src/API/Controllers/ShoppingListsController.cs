using Microsoft.AspNetCore.Mvc;
using Domain.Interfaces.Services;
using DTOs.DTOs.Requests;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShoppingListsController : ControllerBase
{
    private readonly IShoppingListService _shoppingListService;

    public ShoppingListsController(IShoppingListService shoppingListService)
    {
        _shoppingListService = shoppingListService;
    }

    [HttpGet("{sessionId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBySessionId(string sessionId)
    {
        var items = await _shoppingListService.GetShoppingListBySessionIdAsync(sessionId);
        return Ok(items);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddItem([FromBody] CreateShoppingListDTO dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var item = await _shoppingListService.AddItemToShoppingListAsync(dto);
        return StatusCode(StatusCodes.Status201Created, item);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateItem(int id, [FromBody] UpdateShoppingListDTO dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var item = await _shoppingListService.UpdateShoppingListItemAsync(id, dto);
        if (item == null) return NotFound(new { message = $"Item with ID {id} not found" });
        return Ok(item);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveItem(int id)
    {
        var result = await _shoppingListService.RemoveItemFromShoppingListAsync(id);
        if (!result) return NotFound(new { message = $"Item with ID {id} not found" });
        return NoContent();
    }

    [HttpDelete("clear/{sessionId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ClearList(string sessionId)
    {
        var result = await _shoppingListService.ClearShoppingListAsync(sessionId);
        if (!result) return NotFound(new { message = $"Shopping list for session {sessionId} not found or empty" });
        return NoContent();
    }
}
