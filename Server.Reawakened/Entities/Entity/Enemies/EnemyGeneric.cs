﻿using Server.Reawakened.Entities.AIBehavior;
using Server.Reawakened.Rooms;
using Server.Reawakened.Rooms.Extensions;
using Server.Reawakened.Rooms.Models.Entities;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Server.Reawakened.Entities.Entity.Enemies;
public class EnemyGeneric : Enemy
{
    public EnemyGeneric(Room room, int entityId, BaseComponent baseEntity) : base(room, entityId, baseEntity) {}
}
