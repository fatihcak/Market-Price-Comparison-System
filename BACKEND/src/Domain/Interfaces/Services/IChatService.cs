using System.Threading.Tasks;
using DTOs.DTOs.Responses;

namespace Domain.Services
{
    public interface IChatService
    {
        Task<ChatResponseDto> GetChatResponseAsync(string userMessage);
    }
}
