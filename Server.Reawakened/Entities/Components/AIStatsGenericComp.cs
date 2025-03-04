﻿using Server.Reawakened.Rooms.Models.Entities;

namespace Server.Reawakened.Entities.Components;
public class AIStatsGenericComp : Component<AI_Stats_Generic>
{
    public float Patrol_DistanceX => ComponentData.Patrol_DistanceX;
    public float Patrol_DistanceY => ComponentData.Patrol_DistanceY;
    public float Patrol_InitialProgressRatio => ComponentData.Patrol_InitialProgressRatio;
    public int Patrol_ForceDirectionX => ComponentData.Patrol_ForceDirectionX;
    public bool Aggro_UseAttackBeyondPatrolLine => ComponentData.Aggro_UseAttackBeyondPatrolLine;

    public override void InitializeComponent()
    {
    }
}
