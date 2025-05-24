using Chattrix.Application.Interfaces;
using Chattrix.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Chattrix.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _users;

    public UserController(IUserService users)
    {
        _users = users;
    }

    [HttpPost("status")] 
    public async Task<IActionResult> SetStatus(string user, UserStatus status, CancellationToken cancellationToken)
    {
        await _users.SetStatusAsync(user, status, cancellationToken);
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
