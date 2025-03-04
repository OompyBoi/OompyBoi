﻿using A2m.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Server.Base.Core.Extensions;
using Server.Reawakened.Configs;
using Server.Reawakened.XMLs.Abstractions;
using Server.Reawakened.XMLs.BundlesEdit;
using Server.Reawakened.XMLs.BundlesInternal;
using Server.Reawakened.XMLs.Enums;
using Server.Reawakened.XMLs.Extensions;
using System.Reflection;
using System.Xml;

namespace Server.Reawakened.XMLs.Bundles;

public class ItemCatalog : ItemHandler, ILocalizationXml<ItemCatalog>
{
    public string BundleName => "ItemCatalog";
    public string LocalizationName => "ItemCatalogDict_en-US";
    public BundlePriority Priority => BundlePriority.Medium;

    public ILogger<ItemCatalog> Logger { get; set; }
    public IServiceProvider Services { get; set; }

    private Dictionary<string, int> _itemNameDict;
    private Dictionary<ItemCategory, XmlNode> _itemCategories;
    private Dictionary<ItemCategory, Dictionary<ItemSubCategory, XmlNode>> _itemSubCategories;

    public Dictionary<int, ItemDescription> Items;

    public ItemCatalog() : base(null)
    {
    }

    public void InitializeVariables()
    {
        this.SetField<ItemHandler>("_isDisposed", false);
        this.SetField<ItemHandler>("_initDescDone", false);
        this.SetField<ItemHandler>("_initLocDone", false);

        this.SetField<ItemHandler>("_localizationDict", new Dictionary<int, string>());
        this.SetField<ItemHandler>("_itemDescriptionCache", new Dictionary<int, ItemDescription>());
        this.SetField<ItemHandler>("_pendingRequests", new Dictionary<int, ItemDescriptionRequest>());

        _itemNameDict = [];
        _itemCategories = [];
        _itemSubCategories = [];

        Items = [];
    }

    public void EditLocalization(XmlDocument xml)
    {
        _itemNameDict.Clear();

        var dicts = xml.SelectNodes("/ItemCatalogDict/text");

        if (dicts != null)
        {
            var internalCatalog = Services.GetRequiredService<InternalItem>();

            ReadLocalizationXml(xml.WriteToString());
            var localization = this.GetField<ItemHandler>("_localizationDict") as Dictionary<int, string>;

            foreach (XmlNode aNode in dicts)
            {
                if (aNode.Attributes == null)
                    continue;

                var idAttribute = aNode.Attributes["id"];

                if (idAttribute == null)
                    continue;

                var local = int.Parse(idAttribute.InnerText);

                _itemNameDict.TryAdd(aNode.InnerText, local);
            }

            foreach (XmlNode itemCatalogNode in xml.ChildNodes)
            {
                if (!(itemCatalogNode.Name == "ItemCatalogDict")) continue;

                foreach (var item in internalCatalog.Descriptions)
                {
                    var tryGetDict = _itemNameDict.FirstOrDefault(x => x.Value == item.Key);

                    if (!string.IsNullOrEmpty(tryGetDict.Key))
                    {
                        Logger.LogError("Item already exists: {Name} (desc key: {ItemId})", tryGetDict.Key, item.Key);
                        continue;
                    }

                    if (_itemNameDict.ContainsKey(item.Value))
                    {
                        Logger.LogTrace("Item description already exists: {Name}", item.Value);
                        continue;
                    }

                    _itemNameDict.Add(item.Value, AddDictIfNotExists(xml, itemCatalogNode, item.Key, item.Value, localization));
                }
            }
        }
    }

    private static int AddDictIfNotExists(XmlDocument xml, XmlNode node, int nameId,
        string text, Dictionary<int, string> dictList)
    {
        var tryGetDict = dictList.FirstOrDefault(x => x.Value == text);

        if (!string.IsNullOrEmpty(tryGetDict.Value))
            return tryGetDict.Key;

        var vendorElement = xml.CreateElement("text");

        vendorElement.SetAttribute("id", nameId.ToString());
        vendorElement.InnerText = text;

        node.AppendChild(vendorElement);

        return nameId;
    }

    public void ReadLocalization(string xml) =>
        ReadLocalizationXml(xml.ToString());

    public void EditDescription(XmlDocument xml)
    {
        _itemCategories.Clear();
        _itemSubCategories.Clear();

        var internalCatalog = Services.GetRequiredService<InternalItem>();
        var editCatalog = Services.GetRequiredService<EditItem>();
        var config = Services.GetRequiredService<ServerRConfig>();

        var items = new Dictionary<int, string>();

        foreach (XmlNode catalogs in xml.ChildNodes)
        {
            if (!(catalogs.Name == "Catalog")) continue;

            foreach (XmlNode category in catalogs.ChildNodes)
            {
                if (!(category.Name == "ItemCategory")) continue;

                var itemCategory = ItemCategory.Unknown;

                foreach (XmlAttribute categoryAttributes in category.Attributes)
                    if (categoryAttributes.Name == "id")
                        itemCategory = (ItemCategory)int.Parse(categoryAttributes.Value);

                _itemCategories.TryAdd(itemCategory, category);
                _itemSubCategories.TryAdd(itemCategory, []);

                foreach (XmlNode subCategories in category.ChildNodes)
                {
                    if (!(subCategories.Name == "ItemSubcategory")) continue;

                    var subCategory = ItemSubCategory.Unknown;

                    foreach (XmlAttribute subCategoryAttributes in subCategories.Attributes)
                        if (subCategoryAttributes.Name == "id")
                            subCategory = (ItemSubCategory)int.Parse(subCategoryAttributes.Value);

                    _itemSubCategories[itemCategory].TryAdd(subCategory, subCategories);

                    foreach (XmlNode item in subCategories.ChildNodes)
                    {
                        if (!(item.Name == "Item")) continue;

                        var id = -1;
                        var name = string.Empty;

                        foreach (XmlAttribute itemAttributes in item.Attributes)
                        {
                            switch (itemAttributes.Name)
                            {
                                case "id":
                                    id = int.Parse(itemAttributes.Value);
                                    break;
                                case "name":
                                    name = itemAttributes.Value;
                                    break;
                            }
                        }

                        items.Add(id, name);

                        if (editCatalog.EditedItemAttributes[config.GameVersion].TryGetValue(name, out var editedAttributes))
                            foreach (XmlAttribute itemAttributes in item.Attributes)
                                if (editedAttributes.TryGetValue(itemAttributes.Name, out var value))
                                    itemAttributes.Value = value;
                    }
                }
            }

            var smallestItemId = 0;

            foreach (var itemKVP in internalCatalog.Items)
            {
                var item = itemKVP.Value;

                if (!_itemCategories.TryGetValue(item.CategoryId, out var categoryNode))
                {
                    var category = xml.CreateElement("ItemCategory");

                    category.SetAttribute("id", ((int)item.CategoryId).ToString());
                    category.SetAttribute("name", Enum.GetName(item.CategoryId));

                    var node = catalogs.AppendChild(category);
                    categoryNode = node;
                    _itemCategories.Add(item.CategoryId, categoryNode);
                    _itemSubCategories.TryAdd(item.CategoryId, []);
                }

                var itemCategory = categoryNode;

                if (!_itemSubCategories[item.CategoryId].TryGetValue(item.SubCategoryId, out var name))
                {
                    var subCategory = xml.CreateElement("ItemSubcategory");

                    subCategory.SetAttribute("id", ((int)item.SubCategoryId).ToString());
                    subCategory.SetAttribute("name", Enum.GetName(item.SubCategoryId));

                    var node = itemCategory.AppendChild(subCategory);
                    name = node;
                    _itemSubCategories[item.CategoryId].Add(item.SubCategoryId, name);
                }

                var itemSubCategory = name;

                var itemElement = xml.CreateElement("Item");

                var storeType = string.Empty;

                if (item.Store == StoreType.FrontStore)
                    storeType = "Front Store";
                else if (item.Store == StoreType.BackStore)
                    storeType = "Back Store";

                var itemId = item.ItemId;

                if (items.TryGetValue(itemId, out var itemName))
                {
                    Logger.LogError("An item with id '{Id}' already exists! (Conflicts {Name} and {Name2})", itemId, item.ItemName, itemName);
                    continue;
                }

                if (itemId == -1)
                {
                    itemId = items.Keys.ToList().FindSmallest(smallestItemId);
                    items.Add(itemId, item.ItemName);
                    smallestItemId = itemId;
                }

                itemElement.SetAttribute("action_type", Enum.GetName(item.ItemActionType));
                itemElement.SetAttribute("bind_type", Enum.GetName(item.Binding));
                itemElement.SetAttribute("cooldown_time", item.CooldownTime.ToString());
                itemElement.SetAttribute("currency", Enum.GetName(item.Currency));
                itemElement.SetAttribute("discounted_from", item.DiscountedFrom == new DateTime(0L) ? "None" : item.DiscountedFrom.ToString());
                itemElement.SetAttribute("discounted_to", item.DiscountedTo == new DateTime(0L) ? "None" : item.DiscountedTo.ToString());
                itemElement.SetAttribute("global_level", item.LevelRequired.ToString());
                itemElement.SetAttribute("id", itemId.ToString());
                itemElement.SetAttribute("ingamename", _itemNameDict[item.ItemName].ToString());
                itemElement.SetAttribute("item_level", item.Level.ToString());
                // name
                itemElement.SetAttribute("prefab", item.PrefabName.ToString());
                // prefab type
                itemElement.SetAttribute("price", item.RegularPrice.ToString());
                itemElement.SetAttribute("price_discount", item.DiscountPrice.ToString());
                itemElement.SetAttribute("production_status", Enum.GetName(item.ProductionStatus));
                itemElement.SetAttribute("rarity", Enum.GetName(item.Rarity));
                itemElement.SetAttribute("recipe_parent_item_id", item.RecipeParentItemID.ToString());
                itemElement.SetAttribute("sell_price", item.SellPrice.ToString());
                itemElement.SetAttribute("stock_priority", item.StockPriority.ToString());
                itemElement.SetAttribute("store_type", storeType);
                itemElement.SetAttribute("tribe", Enum.GetName(item.Tribe));
                itemElement.SetAttribute("unique_in_inventory", item.UniqueInInventory ? "true" : "false");

                itemElement.SetAttribute("sell_count", item.SellCount.ToString());
                itemElement.SetAttribute("ingamedescription", _itemNameDict[item.DescriptionText].ToString());
                itemElement.SetAttribute("special_display_prefab", item.SpecialDisplayPrefab.ToString());
                itemElement.SetAttribute("member_only", item.MemberOnly ? "true" : "false");
                itemElement.SetAttribute("delay_use_duration", item.DelayUseDuration.ToString());
                itemElement.SetAttribute("loot_id", item.LootId.ToString());
                itemElement.SetAttribute("release_date", item.ReleaseDate == DateTime.UnixEpoch ? "None" : item.ReleaseDate.ToString());

                if (item.ItemEffects.Count > 0)
                {
                    var itemEffectsElement = xml.CreateElement("ItemEffects");

                    foreach (var effect in item.ItemEffects)
                    {
                        var effectElement = xml.CreateElement("Effect");

                        effectElement.SetAttribute("type", Enum.GetName(effect.Type));
                        effectElement.SetAttribute("value", effect.Value.ToString());
                        effectElement.SetAttribute("duration", effect.Duration.ToString());

                        itemEffectsElement.AppendChild(effectElement);
                    }

                    itemElement.AppendChild(itemEffectsElement);
                }

                itemSubCategory.AppendChild(itemElement);
            }
        }
    }

    public void ReadDescription(string xml) =>
        ReadDescriptionXml(xml);

    public void FinalizeBundle()
    {
        var field = typeof(GameGlobals).GetField("_itemHandler",
                    BindingFlags.Static |
                    BindingFlags.NonPublic);

        field.SetValue(null, this);

        Items = (Dictionary<int, ItemDescription>)this.GetField<ItemHandler>("_itemDescriptionCache");
    }
}
