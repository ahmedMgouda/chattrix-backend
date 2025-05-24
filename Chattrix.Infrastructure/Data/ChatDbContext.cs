using Chattrix.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Chattrix.Infrastructure.Data;

public class ChatDbContext : DbContext
{
    public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options) { }

    public DbSet<ChatMessageEntity> Messages => Set<ChatMessageEntity>();
    public DbSet<ChatConversationEntity> Conversations => Set<ChatConversationEntity>();
    public DbSet<UserProfileEntity> Users => Set<UserProfileEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChatMessageEntity>().HasKey(m => m.Id);
        modelBuilder.Entity<ChatConversationEntity>().HasKey(c => c.Id);
        modelBuilder.Entity<UserProfileEntity>().HasKey(u => u.User);
        modelBuilder.Entity<UserProfileEntity>()
            .Property(u => u.Status)
            .HasConversion<int>();
    }
}
