﻿using Microsoft.Extensions.Logging;
using Server.Base.Accounts.Enums;
using Server.Base.Accounts.Helpers;
using Server.Base.Core.Models;
using Server.Base.Network;
using Server.Base.Network.Services;
using System.Globalization;

namespace Server.Base.Accounts.Models;

public class Account : PersistantData, INetStateData
{
    public AccessLevel AccessLevel { get; set; }

    public GameMode GameMode { get; set; }

    public DateTime Created { get; set; }

    public int Flags { get; set; }

    public string[] IpRestrictions { get; set; }

    public DateTime LastLogin { get; set; }

    public string[] LoginIPs { get; set; }

    public string Password { get; set; }

    public List<AccountTag> Tags { get; set; }

    public string Username { get; set; }

    public Account()
    {
    }

    public Account(string username, string password, int userId, PasswordHasher hasher)
    {
        Username = username;
        Password = hasher.GetPassword(username, password);
        AccessLevel = AccessLevel.Player;
        GameMode = GameMode.Spectator;
        Created = DateTime.UtcNow;
        LastLogin = DateTime.UtcNow;
        IpRestrictions = Array.Empty<string>();
        LoginIPs = Array.Empty<string>();
        Tags = new List<AccountTag>();
        UserId = userId;
    }

    public void RemovedState(NetState state, NetStateHandler handler, ILogger logger) =>
        logger.LogError("Disconnected. [{Count} Online] [{Username}]", handler.Instances.Count, Username);
}
