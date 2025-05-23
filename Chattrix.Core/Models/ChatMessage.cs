namespace Chattrix.Core.Models;

public record ChatMessage(Guid Id, string User, string Content, DateTime Timestamp);

