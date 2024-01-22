﻿using Microsoft.Extensions.Logging;
using Server.Reawakened.Entities.Components;
using Server.Reawakened.Network.Protocols;
using Server.Reawakened.Players.Extensions;
using Server.Reawakened.XMLs.Bundles;
using static NPCController;

namespace Protocols.External._n__NpcHandler;

public class ChooseQuestReward : ExternalProtocol
{
    public override string ProtocolName => "nh";

    public ILogger<ChooseQuestReward> Logger { get; set; }
    public QuestCatalog Catalog { get; set; }

    public override void Run(string[] message)
    {
        var vendorId = int.Parse(message[5]);
        var questId = int.Parse(message[6]);
        var itemId = int.Parse(message[7]);
        var questRewardId = int.Parse(message[8]);

        if (itemId != -1)
            Logger.LogError("[Vendor {NpcId}] Unknown quest item reward: {ItemId}", vendorId, itemId);

        if (questRewardId != -1)
            Logger.LogError("[Vendor {NpcId}] Unknown quest reward id: {RewardId}", vendorId, questRewardId);

        var quest = Catalog.QuestCatalogs[questId];

        Player.AddBananas(quest.BananaReward);
        Player.AddReputation(quest.ReputationReward);

        foreach (var item in quest.RewardItems)
            Player.AddItem(item.Key, item.Value);

        Player.SendUpdatedInventory(false);
    }
}
