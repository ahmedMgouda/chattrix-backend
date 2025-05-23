namespace Chattrix.Core.Models;

/// <summary>
/// Basic user profile for status and block list management.
/// </summary>
public class UserProfile
{
    public string User { get; }
    public UserStatus Status { get; set; } = UserStatus.Available;
    public HashSet<string> BlockedUsers { get; } = new();

    public UserProfile(string user)
    {
        User = user;
    }
}
