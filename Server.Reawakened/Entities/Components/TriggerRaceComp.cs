﻿using A2m.Server;
using Server.Reawakened.Entities.AbstractComponents;
using Server.Reawakened.Players;
using Server.Reawakened.Players.Extensions;

namespace Server.Reawakened.Entities.Components;

public class TriggerRaceComp : TriggerStatueComp<TriggerRace>
{
    public string StartRaceSpawnPoint => ComponentData.StartRaceSpawnPoint;
    public string[] TargetToEnableDelayedLevelEditorIDs => ComponentData.TargetToEnableDelayedLevelEditorIDs;
    public string[] TargetToResetLevelEditorIDs => ComponentData.TargetToResetLevelEditorIDs;

    public bool IsMinigameStarted;
    public bool HasMinigameStartedBefore;

    public override object[] GetInitData(Player player) => [IsMinigameStarted ? 1 : 0, HasMinigameStartedBefore ? 1 : 0];

    public override void InitializeComponent()
    {
        base.InitializeComponent();

        IsMinigameStarted = false;
        HasMinigameStartedBefore = false;
    }

    public override void Triggered(Player player, bool isSuccess, bool isActive)
    {
        if (isActive)
        {
            IsMinigameStarted = true;

            if (!HasMinigameStartedBefore)
                HasMinigameStartedBefore = true;
        }
        else
        {
            IsMinigameStarted = false;

            player.CheckObjective(ObjectiveEnum.MinigameMedal, Room.LevelInfo.LevelId.ToString(), Room.LevelInfo.Name, 1, QuestCatalog);
        }
    }
}
