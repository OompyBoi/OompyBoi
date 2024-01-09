﻿using A2m.Server;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Server.Base.Accounts.Models;
using Server.Base.Core.Abstractions;
using Server.Base.Core.Services;
using Server.Base.Worlds.Services;
using Server.Reawakened.Chat.Models;
using Server.Reawakened.Configs;
using Server.Reawakened.Entities.Components;
using Server.Reawakened.Entities.Entity;
using Server.Reawakened.Network.Extensions;
using Server.Reawakened.Players;
using Server.Reawakened.Players.Extensions;
using Server.Reawakened.Rooms.Extensions;
using Server.Reawakened.Rooms.Services;
using Server.Reawakened.XMLs.Bundles;
using System.Text.RegularExpressions;
using UnityEngine;
using static LeaderBoardTopScoresJson;

namespace Server.Reawakened.Chat.Services;

public partial class ChatCommands(ItemCatalog itemCatalog, ServerRConfig config, ILogger<ServerConsole> logger,
    WorldHandler worldHandler, WorldGraph worldGraph, IHostApplicationLifetime appLifetime, AutoSave saves) : IService
{
    private readonly Dictionary<string, ChatCommand> commands = [];

    public void Initialize() => appLifetime.ApplicationStarted.Register(RunChatListener);

    public void RunChatListener()
    {
        logger.LogDebug("Setting up chat commands");

        AddCommand(new ChatCommand("changeName", "[first] [middle] [last]", ChangeName));
        AddCommand(new ChatCommand("unlockHotbar", "[petSlot 1 (true) / 0 (false)]", AddHotbar));
        AddCommand(new ChatCommand("giveItem", "[itemId] [amount]", AddItem));
        AddCommand(new ChatCommand("badgePoints", "[badgePoints]", BadgePoints));
        AddCommand(new ChatCommand("tp", "[X] [Y] [backPlane]", Teleport));
        AddCommand(new ChatCommand("levelUp", "[newLevel]", LevelUp));
        AddCommand(new ChatCommand("itemKit", "[itemKit]", ItemKit));
        AddCommand(new ChatCommand("cashKit", "[cashKit]", CashKit));
        AddCommand(new ChatCommand("warp", "[levelId]", ChangeLevel));
        AddCommand(new ChatCommand("discoverTribes", "", DiscoverTribes));
        AddCommand(new ChatCommand("openDoors", "", OpenDoors));
        AddCommand(new ChatCommand("godmode", "", GodMode));
        AddCommand(new ChatCommand("save", "", SaveLevel));
        AddCommand(new ChatCommand("openVines", "", OpenVines));
        AddCommand(new ChatCommand("getPlayerId", "[id]", GetPlayerId));
        AddCommand(new ChatCommand("initEnemies", "", InitEnemies));
        //AddCommand(new ChatCommand("forceSpawners", "", ForceSpawners));
        //AddCommand(new ChatCommand("getRoomEntityList", "", GetRoomEntityList));

        logger.LogInformation("See chat commands by running {ChatCharStart}help", config.ChatCommandStart);
    }

    public void RunCommand(Player player, string[] args)
    {
        var name = args.FirstOrDefault();

        if (name != null && commands.TryGetValue(name, out var value))
        {
            Log(
                !value.CommandMethod(player, args)
                    ? $"Usage: {config.ChatCommandStart}{value.Name} {value.Arguments}"
                    : "Successfully run command!", player);
        }
        else
        {
            DisplayHelp(player);
        }
    }

    private static void Log(string logMessage, Player player) =>
        player.Chat(CannedChatChannel.Tell, "Console", logMessage);

    public void DisplayHelp(Player player)
    {
        Log("Chat Commands:", player);

        foreach (var command in commands.Values)
        {
            var padding = config.ChatCommandPadding - command.Name.Length;
            if (padding < 0) padding = 0;

            Log(
                $"  {config.ChatCommandStart}{command.Name.PadRight(padding)}" +
                $"{(command.Arguments.Length > 0 ? $" - {command.Arguments}" : "")}",
                player
            );
        }
    }

    public void AddCommand(ChatCommand command) => commands.Add(command.Name, command);

    private bool GodMode(Player player, string[] args)
    {
        var items = config.SingleItemKit
            .Select(itemCatalog.GetItemFromId)
            .ToList();

        foreach (var itemId in config.StackedItemKit)
        {
            var stackedItem = itemCatalog.GetItemFromId(itemId);

            for (var i = 0; i < config.AmountToStack; i++)
                items.Add(stackedItem);
        }

        player.Character.AddKit(items, 1);
        player.SendUpdatedInventory(false);
        player.AddBananas(100000);
        player.AddNCash(100000);
        player.AddPoints();
        player.LevelUp(65, logger);
        player.DiscoverAllTribes();
        player.SendCashUpdate();
        player.AddSlots(true);
        player.Character.Data.CurrentLife = player.Character.Data.MaxLife;
        var health = new Health_SyncEvent(player.GameObjectId.ToString(), player.Room.Time, player.Character.Data.MaxLife, player.Character.Data.MaxLife, "now");
        var t = new StatusEffect_SyncEvent(player.GameObjectId.ToString(), player.Room.Time, (int)ItemEffectType.Healing, 100000, 1, true, player.GameObjectId.ToString(), true);
        player.Room.SendSyncEvent(health);
        player.Room.SendSyncEvent(t);
        return true;
    }
    private bool OpenDoors(Player player, string[] args)
    {
        foreach (var entityComponent in player.Room.Entities.Values.SelectMany(s => s))
        {
            if (entityComponent is TriggerReceiverComp triggerEntity)
            {
                if (config.IgnoredDoors.Contains(entityComponent.PrefabName))
                    continue;

                triggerEntity.Trigger(true);
            }
        }

        return true;
    }

    private bool GetRoomEntityList(Player player, string[] args)
    {
        foreach (var entityComponent in player.Room.Entities.Values.SelectMany(s => s))
            Console.WriteLine(entityComponent);
        return true;
    }

    private bool OpenVines(Player player, string[] args)
    {
        foreach (var entityComponent in player.Room.Entities.Values.SelectMany(s => s))
        {
            if (entityComponent is MysticCharmTargetComp vineEntity)
            {
                vineEntity.Charm(player);
            }
        }

        return true;
    }

    private bool InitEnemies(Player player, string[] args)
    {
        foreach (var entityComponent in player.Room.Entities.Values.SelectMany(s => s))
        {
            if (entityComponent is AIEventControllerComp entity)
            {
                var init = new AIDo_SyncEvent(new SyncEvent(entity.Id.ToString(), SyncEvent.EventType.AIDo, player.Room.Time));
                init.EventDataList.Clear();
                init.EventDataList.Add(entity.Position.X);
                init.EventDataList.Add(entity.Position.Y);
                init.EventDataList.Add(1.0f);
                init.EventDataList.Add(1);
                init.EventDataList.Add("");
                init.EventDataList.Add(entity.Position.X);
                init.EventDataList.Add(entity.Position.Y);
                init.EventDataList.Add(1);
                init.EventDataList.Add(0);
                player.Room.SendSyncEvent(init);
            }
        }
        return true;
    }
    private bool Teleport(Player player, string[] args)
    {
        if (!int.TryParse(args[1], out var xPos)
        || !int.TryParse(args[2], out var yPos)
            || !int.TryParse(args[3], out var zPos))
        {
            Log("Please enter a valid coordinate value.", player);
            return false;
        }

        var z = zPos;

        if (zPos is < 0 or > 1)
        {
            Log("Invalid value for Z, defaulting to 0", player);
            z = 0;
        }

        player.TeleportPlayer(xPos, yPos, z);

        return true;
    }

    private bool DiscoverTribes(Player player, string[] args)
    {
        var character = player.Character;

        player.DiscoverAllTribes();

        Log($"{character.Data.CharacterName} has discovered all tribes!", player);

        return true;
    }

    private bool AddHotbar(Player player, string[] args)
    {
        var hasPet = false;

        if (args.Length == 2)
        {
            if (!int.TryParse(args[1], out var petSlot))
                Log("Unknown pet slot value, defaulting to 0 (false)", player);

            if (petSlot is < 0 or > 1)
                Log("Pet slot value out of range, defaulting to 0 (false)", player);

            hasPet = petSlot == 1;

            Log($"Adding slots ({(hasPet ? "" : "no ")}pet slot)", player);
        }

        player.AddSlots(hasPet);

        Log("Hotbar has been setup! Equip an item or logout to see result.", player);

        return true;
    }

    private bool ItemKit(Player player, string[] args)
    {
        var character = player.Character;

        if (args.Length > 2)
            Log($"Unknown kit amount, defaulting to 1", player);

        var amount = 1;

        if (args.Length == 2)
        {
            if (!int.TryParse(args[1], out var kitAmount))
                Log($"Invalid kit amount, defaulting to 1", player);

            amount = kitAmount;

            if (amount <= 0)
                amount = 1;
        }

        var items = config.SingleItemKit
            .Select(itemCatalog.GetItemFromId)
            .ToList();

        foreach (var itemId in config.StackedItemKit)
        {
            var stackedItem = itemCatalog.GetItemFromId(itemId);

            for (var i = 0; i < config.AmountToStack; i++)
                items.Add(stackedItem);
        }

        character.AddKit(items, amount);

        player.SendUpdatedInventory(false);

        Log($"{character.Data.CharacterName} received {amount} item kit{(amount > 1 ? "s" : "")}!", player);

        return true;
    }

    private static bool BadgePoints(Player player, string[] args)
    {
        player.AddPoints();
        return true;
    }

    private bool CashKit(Player player, string[] args)
    {
        var character = player.Character;

        player.AddBananas(config.CashKitAmount);
        player.AddNCash(config.CashKitAmount);

        Log($"{character.Data.CharacterName} received {config.CashKitAmount} " +
            $"banana{(config.CashKitAmount > 1 ? "s" : "")} & monkey cash!", player);

        return true;
    }

    private static bool ChangeName(Player player, string[] args)
    {
        var character = player.Character;

        if (args.Length < 3)
        {
            Log("Please specify a valid name.", player);
            return false;
        }

        var names = args.Select(name =>
            AlphanumericRegex().Replace(name.ToLower(), "")
        ).ToList();

        var firstName = names[1];
        var secondName = names[2];

        var thirdName = names.Count > 3 ? names[3] : string.Empty;

        if (firstName.Length > 0)
            firstName = char.ToUpper(firstName[0]) + firstName[1..];

        if (secondName.Length > 0)
            secondName = char.ToUpper(secondName[0]) + secondName[1..];

        character.Data.CharacterName = $"{firstName} {secondName}{thirdName}";

        Log($"You have changed your monkey's name to {character.Data.CharacterName}!", player);
        Log("This change will apply only once you've logged out.", player);

        return true;
    }

    private bool ChangeLevel(Player player, string[] args)
    {
        var character = player.Character;

        int levelId;

        if (args.Length != 2 || !int.TryParse(args[1], out var level))
        {
            Log($"Please specify a valid level ID.", player);
            return false;
        }

        levelId = level;

        var levelInfo = worldGraph.GetInfoLevel(levelId);

        if (string.IsNullOrEmpty(levelInfo.Name))
        {
            Log($"Please specify a valid level.", player);
            return false;
        }

        character.SetLevel(levelId, logger);

        var tribe = levelInfo.Tribe;

        player.DiscoverTribe(tribe);
        player.SendLevelChange(worldHandler, worldGraph);

        Log(
            $"Successfully set character {character.Data.CharacterId}'s level to {levelId} '{levelInfo.InGameName}' ({levelInfo.Name})",
            player
        );

        Log($"{character.Data.CharacterName} changed to level {levelId}", player);

        return true;
    }

    private bool LevelUp(Player player, string[] args)
    {
        var character = player.Character;

        if (args.Length != 2 || !int.TryParse(args[1], out var level))
        {
            Log("Invalid level provided, defaulting to max level...", player);
            level = config.MaxLevel;
        }

        var newLevel = level;

        player.LevelUp(newLevel, logger);

        Log($"{character.Data.CharacterName} has leveled up to level {newLevel}!", player);

        return true;
    }

    private bool SaveLevel(Player player, string[] args)
    {
        saves.Save();
        return true;
    }

    private bool AddItem(Player player, string[] args)
    {
        var character = player.Character;

        if (args.Length is < 2 or > 3 || !int.TryParse(args[1], out var itemId))
        {
            Log("Please enter a valid level id.", player);
            return false;
        }

        var amount = 1;

        if (args.Length == 3)
        {
            if (!int.TryParse(args[2], out amount))
                Log("Invalid item count, defaulting to 1...", player);

            if (amount <= 0)
                amount = 1;
        }

        var item = itemCatalog.GetItemFromId(itemId);

        if (item == null)
        {
            Log($"No item with id '{itemId}' could be found.", player);
            return false;
        }

        character.AddItem(item, amount);

        player.SendUpdatedInventory(false);

        Log($"{character.Data.CharacterName} received {item.ItemName} x{amount}", player);

        return true;
    }

    private bool GetPlayerId(Player player, string[] args)
    {
        Log($"{player.CharacterName} has id of {player.GameObjectId}", player);
        return true;
    }

    [GeneratedRegex("[^A-Za-z0-9]+")]
    private static partial Regex AlphanumericRegex();
}
