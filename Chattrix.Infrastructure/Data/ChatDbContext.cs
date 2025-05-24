using System.Text.Json;
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

public class ChatMessageEntity
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public string Sender { get; set; } = string.Empty;
    public string Recipient { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? FilesJson { get; set; }
    public bool IsDelivered { get; set; }
    public bool IsRead { get; set; }
    public bool IsEdited { get; set; }

    public ChatMessage ToModel()
    {
        var files = FilesJson is null ? null :
            JsonSerializer.Deserialize<List<ChatAttachment>>(FilesJson);
        return new ChatMessage(Id, ConversationId, Sender, Recipient, Content, Timestamp, files, IsDelivered, IsRead, IsEdited);
    }

    public static ChatMessageEntity FromModel(ChatMessage model)
    {
        return new ChatMessageEntity
        {
            Id = model.Id,
            ConversationId = model.ConversationId,
            Sender = model.Sender,
            Recipient = model.Recipient,
            Content = model.Content,
            Timestamp = model.Timestamp,
            FilesJson = model.Files is null ? null : JsonSerializer.Serialize(model.Files),
            IsDelivered = model.IsDelivered,
            IsRead = model.IsRead,
            IsEdited = model.IsEdited
        };
    }
}

public class ChatConversationEntity
{
    public Guid Id { get; set; }
    public string User1 { get; set; } = string.Empty;
    public string User2 { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;

    public ChatConversation ToModel() => new(Id, User1, User2, Topic);

    public static ChatConversationEntity FromModel(ChatConversation model) => new()
    {
        Id = model.Id,
        User1 = model.User1,
        User2 = model.User2,
        Topic = model.Topic
    };
}

public class UserProfileEntity
{
    public string User { get; set; } = string.Empty;
    public UserStatus Status { get; set; } = UserStatus.Available;
    public string BlockedUsersJson { get; set; } = "[]";

    public UserProfile ToModel()
    {
        var profile = new UserProfile(User) { Status = Status };
        var blocked = JsonSerializer.Deserialize<HashSet<string>>(BlockedUsersJson) ?? new HashSet<string>();
        foreach (var b in blocked) profile.BlockedUserIds.Add(b);
        return profile;
    }

    public static UserProfileEntity FromModel(UserProfile model)
    {
        return new UserProfileEntity
        {
            User = model.User,
            Status = model.Status,
            BlockedUsersJson = JsonSerializer.Serialize(model.BlockedUserIds)
        };
    }
}
