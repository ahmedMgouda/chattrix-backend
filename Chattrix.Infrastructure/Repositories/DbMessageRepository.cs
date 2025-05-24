using Chattrix.Core.Interfaces;
using Chattrix.Core.Models;
using Chattrix.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Chattrix.Infrastructure.Repositories;

public class DbMessageRepository : IMessageRepository
{
    private readonly ChatDbContext _context;

    public DbMessageRepository(ChatDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ChatMessage message, CancellationToken cancellationToken = default)
    {
        _context.Messages.Add(ChatMessageEntity.FromModel(message));
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ChatMessage message, CancellationToken cancellationToken = default)
    {
        _context.Messages.Update(ChatMessageEntity.FromModel(message));
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Messages.FindAsync([id], cancellationToken);
        if (entity != null)
        {
            _context.Messages.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<ChatMessage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Messages.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        return entity?.ToModel();
    }

    public async Task<IReadOnlyList<ChatMessage>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Messages.AsNoTracking().Select(m => m.ToModel()).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ChatMessage>> GetByConversationAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        return await _context.Messages.AsNoTracking()
            .Where(m => m.ConversationId == conversationId)
            .Select(m => m.ToModel())
            .ToListAsync(cancellationToken);
    }
}
