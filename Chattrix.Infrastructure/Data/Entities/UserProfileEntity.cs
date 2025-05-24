using System;
using System.Collections.Generic;
using System.Text.Json;
using Chattrix.Core.Models;

namespace Chattrix.Infrastructure.Data;

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
