namespace Chattrix.Core.Models;

/// <summary>
/// Basic user profile for status and block list management.
/// </summary>
public class UserProfile
{
    public string User { get; }
    public string Status { get; set; } = string.Empty;
    public HashSet<string> BlockedUsers { get; } = new();

    public UserProfile(string user)
    {
        User = user;
    }
}
