﻿using Server.Reawakened.Players.Models.Character;
using Server.Reawakened.Players.Models.System;

namespace Server.Reawakened.Players.Models;

public class CharacterModel
{
    public CharacterDataModel Data { get; set; }
    public LevelData LevelData { get; set; }
    public Dictionary<int, List<int>> CollectedIdols { get; set; }
    public Dictionary<int, int> ItemsInTrade { get; set; } 
    public List<EmailHeaderModel> Emails { get; set; }

    public CharacterModel()
    {
        CollectedIdols = [];
        Emails = [];
        Data = new CharacterDataModel();
        LevelData = new LevelData();
    }

    public override string ToString() => throw new InvalidOperationException();
}
