using DTOs.DTOs.Requests;
using DTOs.DTOs.Responses;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost]
        public async Task<IActionResult> PostMessage([FromBody] ChatRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                Console.WriteLine($"Model State Error: {errors}");
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(request.Message))
            {
                Console.WriteLine("Message is null or empty");
                return BadRequest("The message cannot be empty.");
            }

            var sessionId = request.SessionId ?? "default_session"; // Fallback to a default if not provided
            var response = await _chatService.GetChatResponseAsync(request.Message, sessionId);

            return Ok(response);
        }
    }
}
