﻿using A2m.Server;
using Microsoft.Extensions.Logging;
using Server.Reawakened.Entities.Components;
using Server.Reawakened.Entities.Enums;
using Server.Reawakened.Players;
using Server.Reawakened.Rooms.Extensions;
using Server.Reawakened.Rooms.Models.Entities;
using Server.Reawakened.Rooms.Models.Planes;
using UnityEngine;

namespace Server.Reawakened.Entities.Entity;
public class ProjectileEntity : Component<ProjectileController>
{
    public new Vector3Model Position;
    public float Speed, LifeTime, StartTime;
    public Player Player;
    public int ProjectileID = 0;
    public UnityEngine.Vector2 CurrentPosition;
    private readonly string _plane;
    public ILogger<ProjectileEntity> Logger { get; set; }

    public ProjectileEntity(Player player, int id, float posX, float posY, float posZ, int direction, float lifeTime, ItemDescription item)
    {
        var isLeft = direction > 0;
        posX += isLeft ? 0.25f : -0.25f;
        posY += 1;
        Speed = isLeft ? 10 : -10;

        Player = player;
        ProjectileID = id;
        Position = new Vector3Model
        {
            X = posX, Y = posY, Z = posZ
        };
        StartTime = player.Room.Time;
        LifeTime = StartTime + lifeTime;
        CurrentPosition = new UnityEngine.Vector2(posX, posY);
        _plane = Position.Z > 10 ? "Plane1" : "Plane0";

        var prj = new LaunchItem_SyncEvent(player.GameObjectId.ToString(), StartTime, posX, posY, posZ, Speed, 0, LifeTime, ProjectileID, item.PrefabName);
        player.Room.SendSyncEvent(prj);
        //Logger.LogInformation("Created Synced Projectile with ID {args1} and lifetime {args2} at position ({args3}, {args4}, {args5})", ProjectileID, LifeTime, Position.X, Position.Y, Position.Z);
    }

    public override void Update()
    {
        base.Update();
        Position.X += Speed * 0.015625f;

        //This is extremely bad code, but this will be updated properly as soon as the ProjectileHit_SyncEvent crash is fixed.
        foreach (var obj in
                 Player.Room.Planes[_plane].GameObjects.Values
                     .Where(obj => Vector3Model.Distance(Position, obj.ObjectInfo.Position) <= 1.5f)
                )
        {
            var isLeft = Speed > 0;

            if (isLeft)
            {
                if (obj.ObjectInfo.Position.X < Position.X)
                    continue;
            }
            else
            {
                if (obj.ObjectInfo.Position.X > Position.X)
                    continue;
            }

            var objectId = obj.ObjectInfo.ObjectId;

            if (Player.Room.Entities.TryGetValue(objectId, out var entityComponents) && Player.Room.Time >= StartTime + 0.1)
                foreach (var component in entityComponents)
                    if (component is TriggerCoopControllerComp wallSwitch && component.PrefabName.Contains("SwitchWall"))
                    {
                        wallSwitch.TriggerInteraction(ActivationType.NormalDamage, Player);
                        ProjectileHit(obj.ObjectInfo.ObjectId.ToString());
                    }
                    else if (component is BreakableEventControllerComp breakableObjEntity)
                    {
                        if (!component.Disposed)
                        {
                            breakableObjEntity.Destroy(Player);
                            ProjectileHit(obj.ObjectInfo.ObjectId.ToString());
                        }
                    }
                    else if (component is InterObjStatusComp enemyEntity && component.Id > 0)
                    {
                        if (!component.Disposed)
                        {
                            enemyEntity.SendDamageEvent(Player);
                            ProjectileHit(obj.ObjectInfo.ObjectId.ToString());
                        }
                    }
    }

        if (LifeTime <= Player.Room.Time)
            ProjectileHit("-1");
    }

    public void ProjectileHit(string hitGoID)
    {
        //Logger.LogInformation("Projectile with ID {args1} destroyed at position ({args2}, {args3}, {args4})", ProjectileID, Position.X, Position.Y, Position.Z);
        var hit = new ProjectileHit_SyncEvent(new SyncEvent(Player.GameObjectId.ToString(), SyncEvent.EventType.ProjectileHit, Player.Room.Time));
        hit.EventDataList.Add(ProjectileID);
        hit.EventDataList.Add(hitGoID);
        hit.EventDataList.Add(0);
        hit.EventDataList.Add(Position.X);
        hit.EventDataList.Add(Position.Y);
        Player.Room.SendSyncEvent(hit);
        Player.Room.Projectiles.Remove(ProjectileID);
    }
}
