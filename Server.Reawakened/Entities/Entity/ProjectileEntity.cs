﻿using A2m.Server;
using Server.Reawakened.Configs;
using Server.Reawakened.Players;
using Server.Reawakened.Rooms.Extensions;
using Server.Reawakened.Rooms.Models.Entities.ColliderType;
using Server.Reawakened.Rooms.Models.Planes;

namespace Server.Reawakened.Entities.Entity;
public class ProjectileEntity : TicklyEntity
{
    private readonly Vector3Model _hitboxPosition;

    public ProjectileEntity(Player player, string id, Vector3Model position, int direction, float lifeTime, ItemDescription item, int damage, Elemental type, ServerRConfig config)
    {
        // Initialize projectile location info
        Player = player;
        ProjectileID = id;
        Position = position;
        PrjPlane = Position.Z > 10 ? "Plane1" : "Plane0";

        // Initialize projectile info
        var isRight = direction > 0;
        Position.X += isRight ? config.ProjectileXOffset : -config.ProjectileXOffset;
        Position.Y += config.ProjectileYOffset;
        SpawnPosition = new Vector3Model { X = Position.X, Y = Position.Y, Z = Position.Z };

        Speed = isRight ? config.ProjectileSpeed : -config.ProjectileSpeed;
        StartTime = player.Room.Time;
        LifeTime = StartTime + lifeTime;
        _hitboxPosition = new Vector3Model { X = Position.X, Y = Position.Y, Z = Position.Z };
        _hitboxPosition.X -= isRight ? 0 : config.ProjectileWidth;

        // Send all information to room
        Collider = new AttackCollider(id, _hitboxPosition, config.ProjectileWidth, config.ProjectileHeight, PrjPlane, player, damage, type, LifeTime);
        var prj = new LaunchItem_SyncEvent(player.GameObjectId.ToString(), StartTime, Position.X, Position.Y, Position.Z, Speed, 0, LifeTime, int.Parse(ProjectileID), item.PrefabName);
        player.Room.SendSyncEvent(prj);
    }

    public override void Hit(string hitGoID)
    {
        //Logger.LogInformation("Projectile with ID {args1} destroyed at position ({args2}, {args3}, {args4})", ProjectileID, Position.X, Position.Y, Position.Z);
        var hit = new ProjectileHit_SyncEvent(new SyncEvent(Player.GameObjectId, SyncEvent.EventType.ProjectileHit, Player.Room.Time));
        hit.EventDataList.Add(int.Parse(ProjectileID));
        hit.EventDataList.Add(hitGoID);
        hit.EventDataList.Add(0);
        hit.EventDataList.Add(Position.X);
        hit.EventDataList.Add(Position.Y);

        Player.Room.SendSyncEvent(hit);
        Player.Room.Projectiles.Remove(ProjectileID);
    }
}
