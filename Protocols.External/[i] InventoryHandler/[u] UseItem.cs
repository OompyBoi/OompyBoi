﻿using A2m.Server;
using Microsoft.Extensions.Logging;
using Server.Reawakened.Network.Extensions;
using Server.Reawakened.Network.Protocols;
using Server.Reawakened.Players;
using Server.Reawakened.Players.Extensions;
using Server.Reawakened.XMLs.Bundles;
using Server.Reawakened.XMLs.BundlesInternal;

namespace Protocols.External._i__InventoryHandler;

public class UseItem : ExternalProtocol
{
    public override string ProtocolName => "iu";

    public ILogger<UseItem> Logger { get; set; }

    public VendorCatalog VendorCatalog { get; set; }
    public ItemCatalogInt ItemCatalog { get; set; }
    public RecipeCatalogInt RecipeCatalog { get; set; }

    public override void Run(string[] message)
    {
        var character = Player.Character;

        var itemId = int.Parse(message[5]);
        var item = ItemCatalog.GetItemFromId(itemId);

        if (item == null)
        {
            Logger.LogError("Could not find item with id {ItemId}", itemId);
            return;
        }

        character.RemoveItem(item, 1);

        switch (item.SubCategoryId)
        {
            case ItemSubCategory.Tailoring:
            case ItemSubCategory.Alchemy:
                var recipe = RecipeCatalog.GetRecipeById(itemId);

                Player.Character.Data.RecipeList.RecipeList.Add(recipe);

                Player.SendXt("cz", recipe);

                break;
            case ItemSubCategory.SuperPack:

                foreach (var pair in VendorCatalog.GetSuperPacksItemsID(itemId))
                {
                    var packItem = ItemCatalog.GetItemFromId(pair);

                    if (packItem == null)
                        continue;

                    character.AddItem(packItem, 3);
                }
                break;
            default:
                Logger.LogWarning("Could not find use for item {ItemId}, type {ItemType}.",
                    itemId, item.SubCategoryId);
                break;
        }

        Player.SendUpdatedInventory(false);
    }

}
