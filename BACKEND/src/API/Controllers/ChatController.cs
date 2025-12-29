using DTOs.DTOs.Requests;
using DTOs.DTOs.Responses;
using Domain.Services;
using Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Asp.Versioning;

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
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(request.Message))
            {
                _logger.LogWarning("Chat request received with empty message");
                return BadRequest("The message cannot be empty.");
            }

            var sessionId = request.SessionId ?? "default_session";
            var response = await _chatService.GetChatResponseAsync(request.Message, sessionId);

            return Ok(response);
        }
    }
}
