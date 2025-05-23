using Chattrix.Core.Models;

namespace Chattrix.Core.Interfaces;

public interface IUserRepository
{
    Task<UserProfile> GetOrCreateAsync(string user, CancellationToken cancellationToken = default);
    Task UpdateAsync(UserProfile profile, CancellationToken cancellationToken = default);
}
