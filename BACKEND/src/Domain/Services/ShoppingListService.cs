using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using DTOs.DTOs.Requests;
using DTOs.DTOs.Responses;

namespace Domain.Services;

public class ShoppingListService : IShoppingListService
{
    private readonly IShoppingListRepository _shoppingListRepository;

    public ShoppingListService(IShoppingListRepository shoppingListRepository)
    {
        _shoppingListRepository = shoppingListRepository;
    }

    public async Task<IEnumerable<ShoppingListResponseDTO>> GetShoppingListBySessionIdAsync(string sessionId)
    {
        var items = await _shoppingListRepository.GetBySessionIdAsync(sessionId);
        return items.Select(MapToResponseDTO);
    }

    public async Task<ShoppingListResponseDTO> AddItemToShoppingListAsync(CreateShoppingListDTO dto)
    {
        var existingItem = await _shoppingListRepository.GetBySessionAndProductIdAsync(dto.SessionId, dto.ProductId);

        if (existingItem != null)
        {
            existingItem.Quantity += dto.Quantity;
            await _shoppingListRepository.UpdateAsync(existingItem);
            return MapToResponseDTO(existingItem);
        }

        var newItem = new UserProductList
        {
            SessionId = dto.SessionId,
            ProductId = dto.ProductId,
            Quantity = dto.Quantity,
            AddedDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        await _shoppingListRepository.AddAsync(newItem);
        return MapToResponseDTO(newItem);
    }

    public async Task<ShoppingListResponseDTO?> UpdateShoppingListItemAsync(int id, UpdateShoppingListDTO dto)
    {
        var item = await _shoppingListRepository.GetByIdAsync(id);
        if (item == null) return null;

        item.Quantity = dto.Quantity;
        await _shoppingListRepository.UpdateAsync(item);

        return MapToResponseDTO(item);
    }

    public async Task<bool> RemoveItemFromShoppingListAsync(int id)
    {
        var item = await _shoppingListRepository.GetByIdAsync(id);
        if (item == null) return false;

        await _shoppingListRepository.DeleteAsync(id);
        return true;
    }

    public async Task<bool> ClearShoppingListAsync(string sessionId)
    {
        var items = await _shoppingListRepository.GetBySessionIdAsync(sessionId);
        if (!items.Any()) return false;

        foreach (var item in items)
        {
            await _shoppingListRepository.DeleteAsync(item.Id);
        }
        return true;
    }

    private static ShoppingListResponseDTO MapToResponseDTO(UserProductList item)
    {
        return new ShoppingListResponseDTO
        {
            Id = item.Id,
            SessionId = item.SessionId,
            ProductId = item.ProductId,
            ProductName = item.Product?.ProductName ?? string.Empty,
            CategoryName = item.Product?.Category?.CategoryName ?? string.Empty,
            Quantity = item.Quantity,
            AddedDate = item.AddedDate
        };
    }
}
