using Microsoft.Extensions.Logging;
using Server.Reawakened.Players;
using Server.Reawakened.Rooms.Extensions;
using Server.Reawakened.Rooms.Models.Entities;

namespace Server.Reawakened.Entities.Components;

public class InterObjStatusComp : Component<InterObjStatus>
{
    public int DifficultyLevel => ComponentData.DifficultyLevel;
    public int GenericLevel => ComponentData.GenericLevel;
    public int Stars => ComponentData.Stars;
    public int MaxHealth => ComponentData.MaxHealth;
    public float LifebarOffsetX => ComponentData.LifeBarOffsetX;
    public float LifebarOffsetY => ComponentData.LifeBarOffsetY;
    public ILogger<InterObjStatusComp> Logger { get; set; }
    public override void InitializeComponent()
    {
        base.InitializeComponent();
    }
}
