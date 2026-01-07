using DTOs.DTOs.Requests;
using DTOs.DTOs.Responses;
using Domain.Services;
using Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Asp.Versioning;
using API.Extensions;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ChatController> _logger;

        public ChatController(IChatService chatService, ILogger<ChatController> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> PostMessage([FromBody] ChatRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                _logger.LogWarning("Model State Error: {Errors}", errors);
                return this.ApiBadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(request.Message))
            {
                _logger.LogWarning("Chat request received with empty message");
                return this.ApiBadRequest("The message cannot be empty.");
            }

            var sessionId = request.SessionId ?? "default_session";
            var response = await _chatService.GetChatResponseAsync(request.Message, sessionId);

            return this.ApiOk(response);
        }

        [HttpPost("stream")]
        public async Task StreamMessage([FromBody] ChatRequestDto request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                _logger.LogWarning("Model State Error: {Errors}", errors);
                Response.StatusCode = 400;
                await Response.WriteAsync($"{{\"error\": \"{errors}\"}}", cancellationToken);
                return;
            }

            if (string.IsNullOrWhiteSpace(request.Message))
            {
                _logger.LogWarning("Chat request received with empty message");
                Response.StatusCode = 400;
                await Response.WriteAsync("{\"error\": \"The message cannot be empty.\"}", cancellationToken);
                return;
            }

            var sessionId = request.SessionId ?? Guid.NewGuid().ToString();

            // Set headers for Server-Sent Events
            Response.Headers.Append("Content-Type", "text/event-stream");
            Response.Headers.Append("Cache-Control", "no-cache");
            Response.Headers.Append("Connection", "keep-alive");

            try
            {
                await foreach (var chunk in _chatService.GetChatResponseStreamAsync(request.Message, sessionId, cancellationToken))
                {
                    await Response.WriteAsync($"data: {chunk}\n\n", cancellationToken);
                    await Response.Body.FlushAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during streaming response");
                await Response.WriteAsync("data: {\"error\": \"Streaming error occurred\"}\n\n", cancellationToken);
            }
        }
    }
}

