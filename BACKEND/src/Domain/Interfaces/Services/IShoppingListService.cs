using DTOs.DTOs.Requests;
using DTOs.DTOs.Responses;

namespace Domain.Interfaces.Services;

public interface IShoppingListService
{
    Task<IEnumerable<ShoppingListResponseDTO>> GetShoppingListBySessionIdAsync(string sessionId);
    Task<ShoppingListResponseDTO> AddItemToShoppingListAsync(CreateShoppingListDTO dto);
    Task<ShoppingListResponseDTO?> UpdateShoppingListItemAsync(int id, UpdateShoppingListDTO dto);
    Task<bool> RemoveItemFromShoppingListAsync(int id);
    Task<bool> ClearShoppingListAsync(string sessionId);
}
