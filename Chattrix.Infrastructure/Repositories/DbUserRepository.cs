using Chattrix.Core.Interfaces;
using Chattrix.Core.Models;
using Chattrix.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Chattrix.Infrastructure.Repositories;

public class DbUserRepository : IUserRepository
{
    private readonly ChatDbContext _context;

    public DbUserRepository(ChatDbContext context)
    {
        _context = context;
    }

    public async Task<UserProfile> GetOrCreateAsync(string user, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Users.FindAsync(new object[] { user }, cancellationToken);
        if (entity == null)
        {
            entity = new UserProfileEntity { User = user };
            _context.Users.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
        return entity.ToModel();
    }

    public async Task UpdateAsync(UserProfile profile, CancellationToken cancellationToken = default)
    {
        _context.Users.Update(UserProfileEntity.FromModel(profile));
        await _context.SaveChangesAsync(cancellationToken);
    }
}
