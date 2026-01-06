using DTOs.DTOs.Responses;
using System.Runtime.CompilerServices;

namespace Domain.Interfaces.Services;

public interface IChatService
{
    Task<ChatResponseDto> GetChatResponseAsync(string userMessage, string sessionId);
    IAsyncEnumerable<string> GetChatResponseStreamAsync(string userMessage, string sessionId, [EnumeratorCancellation] CancellationToken cancellationToken = default);
}
