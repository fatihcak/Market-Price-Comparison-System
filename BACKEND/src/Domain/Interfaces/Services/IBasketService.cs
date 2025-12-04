using DTOs.DTOs.Responses;

namespace Domain.Interfaces.Services;

public interface IBasketService
{
    Task<IEnumerable<BasketComparisonDTO>> CompareBasketAsync(string sessionId);
    Task AddToBasketAsync(string sessionId, int productId, int quantity);
    Task RemoveFromBasketAsync(string sessionId, int productId);
    Task<IEnumerable<UserProductListDTO>> GetBasketAsync(string sessionId);
    Task ClearBasketAsync(string sessionId);
}
