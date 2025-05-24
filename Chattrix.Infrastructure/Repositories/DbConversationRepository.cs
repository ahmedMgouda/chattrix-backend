using Chattrix.Core.Interfaces;
using Chattrix.Core.Models;
using Chattrix.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Chattrix.Infrastructure.Repositories;

public class DbConversationRepository : IConversationRepository
{
    private readonly ChatDbContext _context;

    public DbConversationRepository(ChatDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ChatConversation conversation, CancellationToken cancellationToken = default)
    {
        _context.Conversations.Add(ChatConversationEntity.FromModel(conversation));
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<ChatConversation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Conversations.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        return entity?.ToModel();
    }

    public async Task<IReadOnlyList<ChatConversation>> GetByUsersAsync(string user1, string user2, CancellationToken cancellationToken = default)
    {
        return await _context.Conversations.AsNoTracking()
            .Where(c => (c.User1 == user1 && c.User2 == user2) || (c.User1 == user2 && c.User2 == user1))
            .Select(c => c.ToModel())
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetContactsAsync(string user, CancellationToken cancellationToken = default)
    {
        return await _context.Conversations.AsNoTracking()
            .Where(c => c.User1 == user || c.User2 == user)
            .Select(c => c.User1 == user ? c.User2 : c.User1)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ChatConversation>> GetForUserAsync(string user, CancellationToken cancellationToken = default)
    {
        return await _context.Conversations.AsNoTracking()
            .Where(c => c.User1 == user || c.User2 == user)
            .Select(c => c.ToModel())
            .ToListAsync(cancellationToken);
    }
}
