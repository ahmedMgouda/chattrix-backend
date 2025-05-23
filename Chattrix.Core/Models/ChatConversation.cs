namespace Chattrix.Core.Models;

/// <summary>
/// Represents a chat conversation between two users with a specific topic.
/// </summary>
/// <param name="Id">Unique conversation identifier.</param>
/// <param name="User1">First participant.</param>
/// <param name="User2">Second participant.</param>
/// <param name="Topic">Conversation topic.</param>
public record ChatConversation(Guid Id, string User1, string User2, string Topic);
