using Chattrix.Application.Interfaces;
using Chattrix.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Chattrix.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpGet]
    public async Task<IEnumerable<ChatMessage>> Get(CancellationToken cancellationToken)
    {
        return await _chatService.GetMessagesAsync(cancellationToken);
    }

    [HttpPost]
    public async Task<IActionResult> Post(string user, string content, CancellationToken cancellationToken)
    {
        await _chatService.SendMessageAsync(user, content, cancellationToken);
        return Ok();
    }
}

