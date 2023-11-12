﻿using A2m.Server;
using Microsoft.Extensions.Logging;
using Server.Reawakened.Players;
using Server.Reawakened.Players.Extensions;
using Server.Reawakened.Rooms.Extensions;
using Server.Reawakened.Rooms.Models.Entities;

namespace Server.Reawakened.Entities;

public class HazardControllerEntity : SyncedEntity<HazardController>
{
    public string HurtEffect => EntityData.HurtEffect;
    public float HurtLength => EntityData.HurtLenght;
    public float InitialDamageDelay => EntityData.InitialDamageDelay;
    public float DamageDelay => EntityData.DamageDelay;
    public bool DeathPlane => EntityData.DeathPlane;
    public string NullifyingEffect => EntityData.NullifyingEffect;
    public bool HitOnlyVisible => EntityData.HitOnlyVisible;
    public float InitialProgressRatio => EntityData.InitialProgressRatio;
    public float ActiveDuration => EntityData.ActiveDuration;
    public float DeactivationDuration => EntityData.DeactivationDuration;
    public float HealthRatioDamage => EntityData.HealthRatioDamage;
    public int HurtSelfOnDamage => EntityData.HurtSelfOnDamage;

    public ILogger<HazardControllerEntity> Logger { get; set; }

    public override object[] GetInitData(Player player) => new object[] { 0 };

    public override void NotifyCollision(NotifyCollision_SyncEvent notifyCollisionEvent, Player player)
    {
        /* seems redundant with the other return statement below? not sure though
        if (HurtEffect == "NoEffect")
        {
            return;
        }
        */

        Enum.TryParse(HurtEffect, true, out ItemEffectType effectType);

        var character = player.Character;

        if (effectType == default)
        {
            if (notifyCollisionEvent.Colliding && notifyCollisionEvent.Message == "HitDamageZone") //probably won't work for until some collisions failing is fixed
                player.ApplyDamageByObject(Room, int.Parse(notifyCollisionEvent.CollisionTarget));

            Logger.LogWarning("No hazard type found for {Type}. Returning...", HurtEffect);
            return;
        }

        var statusEffect = new StatusEffect_SyncEvent(player.GameObjectId.ToString(), Room.Time, (int)effectType,
            0, Convert.ToInt32(HurtLength), true, StoredEntity.GameObject.ObjectInfo.PrefabName, false);

        Room.SendSyncEvent(statusEffect);

        Logger.LogTrace("Triggered status effect for {Character} of {HurtType}", character.Data.CharacterName,
            effectType);

        switch (effectType)
        {
            case ItemEffectType.FireDamage:
                player.ApplyDamageByPercent(Room, .10);
                break;
            default:
                SendEntityMethodUnknown("unran-hazards", "Failed Hazard Event", "Hazard Type Switch",
                    $"Effect Type: {effectType}");
                break;
        }
    }
}
