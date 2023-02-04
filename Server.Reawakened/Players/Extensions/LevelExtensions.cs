﻿using Server.Base.Network;
using Server.Reawakened.Levels.Enums;
using Server.Reawakened.Levels.Models;
using Server.Reawakened.Levels.Services;

namespace Server.Reawakened.Players.Extensions;

public static class LevelExtensions
{
    public static void JoinLevel(this Player player, NetState state, Level level, out JoinReason reason)
    {
        player.CurrentLevel?.RemoveClient(player.PlayerId);
        player.CurrentLevel = level;
        player.CurrentLevel.AddClient(state, out reason);
    }

    public static void QuickJoinLevel(this Player player, int id, NetState state, LevelHandler levelHandler)
    {
        Level newLevel = null;

        try
        {
            newLevel = levelHandler.GetLevelFromId(id);
        }
        catch (NullReferenceException)
        {
        }

        if (newLevel == null)
            return;

        player.JoinLevel(state, newLevel, out _);
    }

    public static int GetLevelId(this Player player) =>
        player.CurrentLevel != null ? player.CurrentLevel.LevelInfo.LevelId : -1;

    public static void SendStartPlay(this Player player, int characterId, NetState state, LevelHandler levelHandler)
    {
        player.SetCharacterSelected(characterId);
        player.SendPlayerData(CharacterInfoType.Detailed, state, levelHandler);
    }

    public static void SendPlayerData(this Player player, CharacterInfoType type, NetState state, LevelHandler levelHandler)
    {
        var level = player.GetCurrentLevel(levelHandler);
        level?.SendCharacterInfoData(state, player, type);
    }

    public static Level GetCurrentLevel(this Player player, LevelHandler levelHandler) =>
        levelHandler.GetLevelFromId(player.GetCurrentCharacter().Level);
}
