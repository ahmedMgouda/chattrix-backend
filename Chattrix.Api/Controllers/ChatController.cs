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

    [HttpPost("start")]
    public async Task<ActionResult<Guid>> Start(string user1, string user2, string topic, CancellationToken cancellationToken)
    {
        var id = await _chatService.StartConversationAsync(user1, user2, topic, cancellationToken);
        return Ok(id);
    }

    [HttpGet("{conversationId}")]
    public Task<IReadOnlyList<ChatMessage>> Get(Guid conversationId, CancellationToken cancellationToken)
    {
        return _chatService.GetMessagesAsync(conversationId, cancellationToken);
    }

    [HttpPost("{conversationId}")]
    public async Task<IActionResult> Post(Guid conversationId, string sender, string content, string? fileName, CancellationToken cancellationToken)
    {
        await _chatService.SendMessageAsync(conversationId, sender, content, fileName, cancellationToken);
        return Ok();
    }

    [HttpGet("{conversationId}/search")]
    public Task<IReadOnlyList<ChatMessage>> Search(Guid conversationId, string term, CancellationToken cancellationToken)
    {
        return _chatService.SearchAsync(conversationId, term, cancellationToken);
    }

    [HttpGet("{conversationId}/files")]
    public Task<IReadOnlyList<string>> Files(Guid conversationId, CancellationToken cancellationToken)
    {
        return _chatService.GetFilesAsync(conversationId, cancellationToken);
    }

    [HttpGet("message/{id}")]
    public Task<ChatMessage?> GetMessage(Guid id, CancellationToken cancellationToken)
    {
        return _chatService.GetMessageAsync(id, cancellationToken);
    }

    [HttpPut("message/{id}")]
    public async Task<IActionResult> Put(Guid id, string content, CancellationToken cancellationToken)
    {
        await _chatService.UpdateMessageAsync(id, content, cancellationToken);
        return NoContent();
    }

    [HttpDelete("message/{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _chatService.DeleteMessageAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPut("message/{id}/delivered")]
    public async Task<IActionResult> Delivered(Guid id, CancellationToken cancellationToken)
    {
        await _chatService.MarkDeliveredAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPut("message/{id}/read")]
    public async Task<IActionResult> Read(Guid id, CancellationToken cancellationToken)
    {
        await _chatService.MarkReadAsync(id, cancellationToken);
        return NoContent();
    }
}
