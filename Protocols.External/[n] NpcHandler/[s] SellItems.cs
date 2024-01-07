﻿using Server.Reawakened.Network.Protocols;
using Server.Reawakened.Players.Extensions;
using Server.Reawakened.XMLs.BundlesInternal;

namespace Protocols.External._n__NpcHandler;
public class SellItems : ExternalProtocol
{
    public override string ProtocolName => "ns";

    public ItemCatalogInt ItemCatalog { get; set; }

    public override void Run(string[] message)
    {
        var character = Player.Character;

        var items = message[6].Split('|');

        foreach (var item in items)
        {
            if (string.IsNullOrEmpty(item))
                continue;

            var args = item.Split(":");
            var itemId = int.Parse(args[0]);
            var amount = int.Parse(args[1]);

            var itemDescription = ItemCatalog.GetItemFromId(itemId);

            character.RemoveItem(itemDescription, amount);

            Player.AddBananas(itemDescription.SellPrice * amount);

            Player.SendUpdatedInventory(false);
        }
    }
}
