﻿using A2m.Server;
using Server.Base.Core.Abstractions;
using Server.Base.Core.Extensions;

namespace Server.Reawakened.Configs;

public class ServerRConfig : IRConfig
{
    public int DefaultChatLevel { get; }
    public bool DefaultMemberStatus { get; }

    public string DefaultSignUpExperience { get; }
    public string DefaultTrackingShortId { get; }

    public int RandomKeyLength { get; }
    public int PlayerCap { get; }
    public int ReservedNameCount { get; }
    public int MaxCharacterCount { get; }

    public int HealingStaff { get; }
    public double HealingStaffHealValue { get; }
    public int DefaultDamage { get; }

    public Dictionary<bool, string> IsBackPlane { get; }

    public int MaxLevel { get; }

    public int DefaultQuest { get; }

    public bool LogSyncState { get; }

    public string LevelSaveDirectory { get; }
    public string LevelDataSaveDirectory { get; }
    public string XMLDirectory { get; }
    public string DataDirectory { get; }

    public string[] DefaultProtocolTypeIgnore { get; }

    public char ChatCommandStart { get; }
    public int ChatCommandPadding { get; }

    public double RoomTickRate { get; }

    public int LogOnLagCount { get; }

    public bool ClearCache { get; }

    public Dictionary<DebugHandler.DebugVariables, bool> DefaultDebugVariables { get; }

    public int[] SingleItemKit { get; }
    public int[] StackedItemKit { get; }
    public int AmountToStack { get; }
    public int CashKitAmount { get; }
    public int HealAmount { get; }

    public double KickAfterTime { get; }

    public bool LogAllSyncEvents { get; }
    public int AccessRights { get; }

    public Dictionary<TribeType, int> TutorialTribe2014 { get; }
    public GameVersion GameVersion { get; set; }

    public string[] IgnoredDoors { get; set; }

    public int MaximumEntitiesToReturnLog { get; set; }
    public string[] EnemyNameSearch { get; set; }
    public string BreakableComponentName { get; set; }
    public string EnemyComponentName { get; set; }
    public string DailyBoxName { get; set; }
    public int HealingStaffID { get; set; }
    public int MysticCharmID { get; set; }
    public float ProjectileSpeed { get; set; }
    public float ProjectileXOffset { get; set; }
    public float ProjectileYOffset { get; set; }
    public float ProjectileWidth { get; set; }
    public float ProjectileHeight { get; set; }
    public float MeleeXOffset { get; set; }
    public float MeleeYOffset { get; set; }
    public float MeleeWidth { get; set; }
    public float MeleeHeight { get; set; }
    public float PlayerWidth { get; set; }
    public float PlayerHeight { get; set; }
    public Dictionary<int, string> TrainingGear { get; set; }

    public List<string> LoadedAssets { get; set; }

    public ServerRConfig()
    {
        LevelSaveDirectory = InternalDirectory.GetDirectory("XMLs/Levels");
        LevelDataSaveDirectory = InternalDirectory.GetDirectory("XMLs/LevelData");
        DataDirectory = InternalDirectory.GetDirectory("XMLs/FormattedData");
        XMLDirectory = InternalDirectory.GetDirectory("XMLs/XMLFiles");

        RoomTickRate = 32;

        RandomKeyLength = 24;
        PlayerCap = 200;
        ReservedNameCount = 4;
        MaxCharacterCount = 3;

        DefaultSignUpExperience = "Console";
        DefaultChatLevel = 3;
        DefaultTrackingShortId = "false";
        DefaultMemberStatus = true;

        DefaultQuest = 802;

        LogSyncState = false;
        DefaultProtocolTypeIgnore = ["ss", "Pp", "ku", "kr"];

        ChatCommandStart = '.';
        ChatCommandPadding = 8;

        LogOnLagCount = 200;

        DefaultDebugVariables = new Dictionary<DebugHandler.DebugVariables, bool>
        {
            { DebugHandler.DebugVariables.Sharder_active, false },
            { DebugHandler.DebugVariables.Sharder_1, false },
            { DebugHandler.DebugVariables.Sharder_2, false },
            { DebugHandler.DebugVariables.Ewallet, true },
            { DebugHandler.DebugVariables.Chat, true },
            { DebugHandler.DebugVariables.BugReport, true },
            { DebugHandler.DebugVariables.Crisp, true },
            { DebugHandler.DebugVariables.Trade, true }
        };

        SingleItemKit =
        [
            394,  // glider
            395,  // grappling hook
            240181867,  // snowboard
            397,  // wooden slingshot
            423,  // golden slingshot
            453,  // kernel blaster
            1009, // snake staff
            584,  // scrying orb
            2978, // wooden sword
            2883, // oak plank helmet
            2886, // oak plank armor
            2880, // oak plank pants
            1232, // black cat burglar mask
            3152, // super monkey pack
            3053, // boom bomb construction kit
            3023, // ladybug warrior costume pack
            3022, // boom bug pack
            2972, // ace pilot pack
            2973, // crimson dragon pack
            2923, // banana box
            3024, // steel sword
            2878 // cadet training sword
        ];

        StackedItemKit =
        [
            396,  // healing staff
            585,  // invisible bomb
            1568, // red apple
            405,  // healing potion
        ];

        AmountToStack = 99;
        CashKitAmount = 100000;

        KickAfterTime = TimeSpan.FromMinutes(5).TotalMilliseconds;

        LogAllSyncEvents = true;
        ClearCache = true;

        AccessRights = (int)UserAccessRight.NoDictionaryChat;

        TutorialTribe2014 = new Dictionary<TribeType, int>
        {
            { TribeType.Shadow, 966 }, // NINJA
            { TribeType.Outlaw, 967 }, // PIRATE
            { TribeType.Wild,   968 }, // ICE
            { TribeType.Bone,   969 }, // OOTU
        };

        GameVersion = GameVersion.v2013;
        IsBackPlane = new Dictionary<bool, string>()
        {
            { true, "Plane1" },
            { false, "Plane0" }
        };

        MaxLevel = 65;
        HealAmount = 100000;

        IgnoredDoors = [
            "PF_GLB_DoorArena01"
        ];

        HealingStaff = 396;
        HealingStaffHealValue = 3.527f;
        DefaultDamage = 10;

        EnemyComponentName = "EnemyController";
        BreakableComponentName = "BreakableEventController";
        DailyBoxName = "Daily";

        EnemyNameSearch = [
            "PF_Critter_Bird",
            "PF_Critter_Fish",
            "PF_Critter_Spider",
            "PF_Spite_Bathog",
            "PF_Spite_Bomber",
            "PF_Spite_Crawler",
            "PF_Spite_Dragon",
            "PF_Spite_Grenadier",
            "PF_Spite_Orchid",
            "PF_Spite_Pincer",
            "PF_Spite_Stomper",
            "Spite_Wasp_Boss01"
        ];

        MaximumEntitiesToReturnLog = 15;
        HealingStaffID = 396;
        MysticCharmID = 398;

        ProjectileSpeed = 10;

        ProjectileXOffset = 0.25f;
        ProjectileYOffset = 0.8f;
        ProjectileHeight = 0.5f;
        ProjectileWidth = 0.5f;

        MeleeXOffset = 0f;
        MeleeYOffset = 0f;
        MeleeHeight = 1f;
        MeleeWidth = 3f;

        PlayerHeight = 1f;
        PlayerWidth = 1f;

        TrainingGear = new Dictionary<int, string>
        {
            { 465, "ABIL_GrapplingHook01" }, // lv_shd_teaser01
            { 466, "ABIL_Glider01" }, // lv_out_teaser01
            { 467, "ABIL_MysticCharm01" }, // lv_bon_teaser01
            { 497, "ABIL_SnowBoard01" }, // lv_wld_teaser01
            { 498, "ABIL_SnowBoard02" }, // lv_wld_teaser01
        };

        LoadedAssets = [];
    }
}
