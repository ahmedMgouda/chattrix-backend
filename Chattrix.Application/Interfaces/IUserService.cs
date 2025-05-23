using Chattrix.Core.Models;

namespace Chattrix.Application.Interfaces;

public interface IUserService
{
    Task SetStatusAsync(string user, UserStatus status, CancellationToken cancellationToken = default);
    Task<UserStatus> GetStatusAsync(string user, CancellationToken cancellationToken = default);
    Task BlockAsync(string user, string blockedUser, CancellationToken cancellationToken = default);
    Task UnblockAsync(string user, string blockedUser, CancellationToken cancellationToken = default);
    Task<bool> IsBlockedAsync(string user, string otherUser, CancellationToken cancellationToken = default);
}
