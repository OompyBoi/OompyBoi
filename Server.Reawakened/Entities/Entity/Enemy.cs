using Server.Base.Core.Extensions;
using Server.Reawakened.Entities.AIBehavior;
using Server.Reawakened.Entities.Components;
using Server.Reawakened.Entities.Stats;
using Server.Reawakened.Players;
using Server.Reawakened.Rooms;
using Server.Reawakened.Rooms.Extensions;
using Server.Reawakened.Rooms.Models.Entities;
using Server.Reawakened.Rooms.Models.Planes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Server.Reawakened.Entities.Entity;
public class Enemy
{
    public bool Init;
    public Room Room;
    public int Id;
    public Vector3 SpawnPosition;
    public Vector3 Position;
    public int Health;
    public Rect DetectionRange;
    public BaseCollider Hitbox;
    public string ParentPlane;

    public AI_Stats_Global_Bank Global;
    public AI_Stats_Generic_Bank Generic;
    public InterObjStatusComp Status;
    public BaseComponent Entity;

    public AIProcessData AiData;
    public GlobalProperties EnemyGlobalProps;
    public AIBaseBehavior Behavior;
    public float PatrolSpeed;
    public float EndPathWaitTime;


    public Enemy(Room room, int entityId, BaseComponent baseEntity)
    {
        var entityList = room.Entities.Values.SelectMany(s => s);
        foreach (var entity in entityList)
        {
            if (entity.Id == Id && entity is AI_Stats_Global_Bank global)
                Global = global;
            else if (entity.Id == Id && entity is AI_Stats_Generic_Bank generic)
                Generic = generic;
            else if (entity.Id == Id && entity is InterObjStatusComp status)
                Status = status;
            else if (entity.Id == Id && entity is EnemyControllerComp enemy)
                Entity = enemy;
        }
        Console.WriteLine(Entity.PrefabName);
        Console.WriteLine(Generic.Patrol_DistanceX);

        Entity = baseEntity;
        Room = room;
        Id = entityId;
        Position = new Vector3(Entity.Position.X, Entity.Position.Y, Entity.Position.Z);
        SpawnPosition = Position;
        ParentPlane = Entity.ParentPlane;
        Console.WriteLine(Entity.Position);


        EnemyGlobalProps = new GlobalProperties(
            Global.Global_DetectionLimitedByPatrolLine,
            Global.Global_BackDetectionRangeX,
            Global.Global_viewOffsetY,
            Global.Global_BackDetectionRangeUpY,
            Global.Global_BackDetectionRangeDownY,
            Global.Global_ShootOffsetX,
            Global.Global_ShootOffsetY,
            Global.Global_FrontDetectionRangeX,
            Global.Global_FrontDetectionRangeUpY,
            Global.Global_FrontDetectionRangeDownY,
            Global.Global_Script,
            Global.Global_ShootingProjectilePrefabName,
            Global.Global_DisableCollision,
            Global.Global_DetectionSourceOnPatrolLine,
            Global.Aggro_AttackBeyondPatrolLine
        );

        AiData = new AIProcessData();
        AiData.SetStats(EnemyGlobalProps);
        AiData.SyncInit_PosX = Position.x;
        AiData.SyncInit_PosY = Position.y;
        AiData.Intern_SpawnPosX = Position.x;
        AiData.Intern_SpawnPosY = Position.y;
        AiData.Intern_SpawnPosZ = Position.z;
        AiData.SyncInit_Dir = Generic.Patrol_ForceDirectionX;
        AiData.SyncInit_ProgressRatio = Generic.Patrol_InitialProgressRatio;

        Hitbox = new BaseCollider(Id, Entity.Position, Entity.Rectangle.Width, Entity.Rectangle.Height, Entity.ParentPlane, Room);
        Room.Colliders.Add(Id, Hitbox);
    }
    public virtual void Initialize()
    {
    }

    public virtual void Update()
    {
        if (!Init)
        {
            // Address magic numbers when we get around to adding enemy effect mods
            Room.SendSyncEvent(AIInit(1, 1, 1));
            // Address first magic number when we get to adding enemy effect mods
            Room.SendSyncEvent(AIDo(1.0f, 1, "", Position.x, Position.y, Generic.Patrol_ForceDirectionX, 0));
            Init = true;
        }

        Behavior.Update(AiData, Room.Time);

        Position = new Vector3(AiData.SyncInit_PosX, AiData.SyncInit_PosY, Position.z);
        Hitbox.Position = Position;
    }

    public virtual string WriteBehaviorList()
    {
        string output = "Idle||";
        List<string> behaviorList = [];
        
        if (Entity.PrefabName.Contains("PF_Critter_Bird"))
        {
            PatrolSpeed = 3.2f;
            EndPathWaitTime = 0;
            behaviorList.Add("Patrol|" + PatrolSpeed + ";" + 0 + ";" + EndPathWaitTime + ";" + Generic.Patrol_DistanceX + ";" + Generic.Patrol_DistanceY + ";" + Generic.Patrol_ForceDirectionX + ";" + Generic.Patrol_InitialProgressRatio + "|");
        }
        else  if (Entity.PrefabName.Contains("PF_Critter_Spider"))
        {
            PatrolSpeed = 5.0f;
            EndPathWaitTime = 2;
            behaviorList.Add("Patrol|" + PatrolSpeed + ";" + 0 + ";" + EndPathWaitTime + ";" + Generic.Patrol_DistanceX + ";" + Generic.Patrol_DistanceY + ";" + Generic.Patrol_ForceDirectionX + ";" + Generic.Patrol_InitialProgressRatio + "|");
        }
        else if (Entity.PrefabName.Contains("PF_Critter_Fish"))
        {
            PatrolSpeed = 3.2f;
            EndPathWaitTime = 2;
            behaviorList.Add("Patrol|" + PatrolSpeed + ";" + 0 + ";" + EndPathWaitTime + ";" + Generic.Patrol_DistanceX + ";" + Generic.Patrol_DistanceY + ";" + Generic.Patrol_ForceDirectionX + ";" + Generic.Patrol_InitialProgressRatio + "|");
        }
        else if (Entity.PrefabName.Contains("PF_Spite_Bathog"))
        {
            PatrolSpeed = 1.8f;
            EndPathWaitTime = 3;
            behaviorList.Add("Patrol|" + PatrolSpeed + ";" + 0 + ";" + EndPathWaitTime + ";" + Generic.Patrol_DistanceX + ";" + Generic.Patrol_DistanceY + ";" + Generic.Patrol_ForceDirectionX + ";" + Generic.Patrol_InitialProgressRatio + "|");
        }
        else if (Entity.PrefabName.Contains("PF_Spite_Bomber"))
        {
            PatrolSpeed = 1.8f;
            EndPathWaitTime = 2;
            behaviorList.Add("Patrol|" + PatrolSpeed + ";" + 0 + ";" + EndPathWaitTime + ";" + Generic.Patrol_DistanceX + ";" + Generic.Patrol_DistanceY + ";" + Generic.Patrol_ForceDirectionX + ";" + Generic.Patrol_InitialProgressRatio + "|");
        }
        else if (Entity.PrefabName.Contains("PF_Spite_Crawler"))
        {
            PatrolSpeed = 1.8f;
            EndPathWaitTime = 3;
            behaviorList.Add("Patrol|" + PatrolSpeed + ";" + 0 + ";" + EndPathWaitTime + ";" + Generic.Patrol_DistanceX + ";" + Generic.Patrol_DistanceY + ";" + Generic.Patrol_ForceDirectionX + ";" + Generic.Patrol_InitialProgressRatio + "|");
            behaviorList.Add("Aggro|" + 3.2 + ";" + Global.Aggro_MoveBeyondTargetDistance + ";" + 0 + ";" + Global.Aggro_AttackBeyondPatrolLine + ";" + 0 + ";" + Global.Global_FrontDetectionRangeUpY + ";" + Global.Global_FrontDetectionRangeDownY + "|");
        }
        else if (Entity.PrefabName.Contains("PF_Spite_Dragon"))
        {
            PatrolSpeed = 1.8f;
            EndPathWaitTime = 2;
            behaviorList.Add("Patrol|" + PatrolSpeed + ";" + 0 + ";" + EndPathWaitTime + ";" + Generic.Patrol_DistanceX + ";" + Generic.Patrol_DistanceY + ";" + Generic.Patrol_ForceDirectionX + ";" + Generic.Patrol_InitialProgressRatio + "|");
        }
        else if (Entity.PrefabName.Contains("PF_Spite_Pincer"))
        {
            PatrolSpeed = 2.4f;
            EndPathWaitTime = 0;
            behaviorList.Add("Patrol|" + PatrolSpeed + ";" + 0 + ";" + EndPathWaitTime + ";" + Generic.Patrol_DistanceX + ";" + Generic.Patrol_DistanceY + ";" + Generic.Patrol_ForceDirectionX + ";" + Generic.Patrol_InitialProgressRatio + "|");
        }
        else if (Entity.PrefabName.Contains("PF_Spite_Stomper"))
        {
            PatrolSpeed = 1.8f;
            EndPathWaitTime = 3;
            behaviorList.Add("Patrol|" + PatrolSpeed + ";" + 0 + ";" + EndPathWaitTime + ";" + Generic.Patrol_DistanceX + ";" + Generic.Patrol_DistanceY + ";" + Generic.Patrol_ForceDirectionX + ";" + Generic.Patrol_InitialProgressRatio + "|");
        }

        foreach (var bah in behaviorList)
        {
            if (behaviorList.Count > 0)
                output = output + "`" + bah;
        }

        Behavior = new AIBehavior_Patrol(new Vector3(SpawnPosition.x, SpawnPosition.y, SpawnPosition.z),
        new Vector3(SpawnPosition.x + Generic.Patrol_DistanceX, SpawnPosition.y + Generic.Patrol_DistanceY, SpawnPosition.z),
        PatrolSpeed,
        EndPathWaitTime);

        return output;
    }

    public bool PlayerInRange(Vector3Model pos)
    {
        if (Position.x - DetectionRange.width / 2 < pos.X && pos.X < Position.x + DetectionRange.width / 2 &&
            Position.y < pos.Y && pos.Y < Position.y + DetectionRange.height)
            return true;
        return false;
    }
    public AIDo_SyncEvent AIDo(float speedFactor, int behaviorId, string args, float targetPosX, float targetPosY, int direction, int awareBool)
    {
        var aiDo = new AIDo_SyncEvent(new SyncEvent(Id.ToString(), SyncEvent.EventType.AIDo, Room.Time));
        aiDo.EventDataList.Clear();
        aiDo.EventDataList.Add(Position.x);
        aiDo.EventDataList.Add(Position.y);
        aiDo.EventDataList.Add(speedFactor);
        aiDo.EventDataList.Add(behaviorId);
        aiDo.EventDataList.Add(args);
        aiDo.EventDataList.Add(targetPosX);
        aiDo.EventDataList.Add(targetPosY);
        aiDo.EventDataList.Add(direction);
        // 0 for false, 1 for true.
        aiDo.EventDataList.Add(awareBool);
        return aiDo;
    }

    public AIInit_SyncEvent AIInit(float healthMod, float sclMod, float resMod)
    {
        int z = 0;
        if (ParentPlane == "Plane1")
            z = 20;

        var aiInit = new AIInit_SyncEvent(Id.ToString(), Room.Time, Position.x, Position.y, z, Position.x, Position.y, Generic.Patrol_InitialProgressRatio,
            Status.MaxHealth, Status.MaxHealth, healthMod, sclMod, resMod, Status.Stars, Status.GenericLevel, EnemyGlobalProps.ToString(), WriteBehaviorList());
        aiInit.EventDataList[2] = Position.x;
        aiInit.EventDataList[3] = Position.y;
        aiInit.EventDataList[4] = Position.z;
        return aiInit;
    }
}
