using Chattrix.Application.Interfaces;
using Chattrix.Core.Models;
using Chattrix.Api.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Chattrix.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _users;
    private readonly IHubContext<UserHub> _hub;

    public UserController(IUserService users, IHubContext<UserHub> hub)
    {
        _users = users;
        _hub = hub;
    }

    [HttpPost("status")] 
    public async Task<IActionResult> SetStatus(string user, UserStatus status, CancellationToken cancellationToken)
    {
        await _users.SetStatusAsync(user, status, cancellationToken);
        await _hub.Clients.All.SendAsync("StatusChanged", user, status, cancellationToken);
        return NoContent();
    }

    [HttpGet("status/{user}")]
    public Task<UserStatus> GetStatus(string user, CancellationToken cancellationToken)
    {
        return _users.GetStatusAsync(user, cancellationToken);
    }

    [HttpPost("block")]
    public async Task<IActionResult> Block(string user, string blocked, CancellationToken cancellationToken)
    {
        await _users.BlockAsync(user, blocked, cancellationToken);
        return NoContent();
    }

    [HttpPost("unblock")]
    public async Task<IActionResult> Unblock(string user, string blocked, CancellationToken cancellationToken)
    {
        await _users.UnblockAsync(user, blocked, cancellationToken);
        return NoContent();
    }
}
