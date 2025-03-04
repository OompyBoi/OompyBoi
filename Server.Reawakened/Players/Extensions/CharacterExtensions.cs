﻿using A2m.Server;
using Microsoft.Extensions.Logging;
using Server.Reawakened.Players.Models;

namespace Server.Reawakened.Players.Extensions;

public static class CharacterExtensions
{
    public static int GetHealthForLevel(int level) => (level - 1) * 270 + 81;

    public static int GetReputationForLevel(int level) => (Convert.ToInt32(Math.Pow(level, 2)) - (level - 1)) * 500;

    public static void SetLevelXp(this CharacterModel characterData, int level, int reputation = 0)
    {
        characterData.Data.GlobalLevel = level;

        characterData.Data.ReputationForCurrentLevel = GetReputationForLevel(level - 1);
        characterData.Data.ReputationForNextLevel = GetReputationForLevel(level);
        characterData.Data.Reputation = reputation;

        characterData.Data.MaxLife = GetHealthForLevel(level);
        characterData.Data.CurrentLife = characterData.Data.MaxLife;
    }

    public static bool HasAddedDiscoveredTribe(this CharacterModel characterData, TribeType tribe)
    {
        if (characterData == null)
            return false;

        if (characterData.Data.TribesDiscovered.TryGetValue(tribe, out var value))
        {
            if (value)
                return false;

            characterData.Data.TribesDiscovered[tribe] = true;
        }
        else
        {
            characterData.Data.TribesDiscovered.Add(tribe, true);
        }

        return true;
    }

    public static void SetLevel(this CharacterModel character, int levelId,
        Microsoft.Extensions.Logging.ILogger logger) =>
        character.SetLevel(levelId, string.Empty, logger);

    public static void SetLevel(this CharacterModel character, int levelId, string spawnId,
        Microsoft.Extensions.Logging.ILogger logger)
    {
        character.LevelData.LevelId = levelId;
        character.LevelData.SpawnPointId = spawnId;

        logger.LogDebug("Set spawn of '{CharacterName}' to spawn {SpawnId}",
            character.Data.CharacterName, spawnId);
    }
}
