﻿using A2m.Server;
using Server.Reawakened.Players;
using Server.Reawakened.Rooms;

namespace Server.Reawakened.Network.Extensions;

public static class ChatExtensions
{
    public static void Chat(this Player player, CannedChatChannel channelId, string sender, string message) =>
        player.SendXt("ae", (int)channelId, sender, message, sender == string.Empty ? "0" : "1");

    public static void Chat(this Room room, CannedChatChannel channelId, string sender, string message)
    {
        foreach (
            var client in
            from client in room.Players.Values
            select client
        )
            client.Chat(channelId, sender, message);
    }
}
