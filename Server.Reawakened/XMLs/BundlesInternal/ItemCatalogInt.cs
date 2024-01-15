﻿using A2m.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Server.Reawakened.XMLs.Abstractions;
using Server.Reawakened.XMLs.Bundles;
using Server.Reawakened.XMLs.Enums;
using Server.Reawakened.XMLs.Extensions;
using System.Xml;

namespace Server.Reawakened.XMLs.BundlesInternal;

public class ItemCatalogInt : IBundledXml
{
    public string BundleName => "ItemCatalogInt";
    public BundlePriority Priority => BundlePriority.High;

    public Microsoft.Extensions.Logging.ILogger Logger { get; set; }
    public IServiceProvider Services { get; set; }

    public List<ItemDescription> Items;
    public Dictionary<int, string> Descriptions;

    public ItemCatalogInt()
    {
    }

    public void InitializeVariables()
    {
        Items = [];
        Descriptions = [];
    }

    public void EditDescription(XmlDocument xml)
    {
    }

    public void ReadDescription(string xml)
    {
        var miscDict = Services.GetRequiredService<MiscTextDictionary>();

        var xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);

        foreach (XmlNode catalogs in xmlDocument.ChildNodes)
        {
            if (!(catalogs.Name == "Catalog")) continue;

            foreach (XmlNode category in catalogs.ChildNodes)
            {
                if (!(category.Name == "ItemCategory")) continue;

                var itemCategory = ItemCategory.Unknown;

                foreach (XmlAttribute categoryAttributes in category.Attributes)
                    if (categoryAttributes.Name == "name")
                        itemCategory = itemCategory.GetEnumValue(categoryAttributes.Value, Logger);

                foreach (XmlNode subCategories in category.ChildNodes)
                {
                    if (!(subCategories.Name == "ItemSubCategory")) continue;

                    var subCategory = ItemSubCategory.Unknown;

                    foreach (XmlAttribute subCategoryAttributes in subCategories.Attributes)
                        if (subCategoryAttributes.Name == "name")
                            subCategory = subCategory.GetEnumValue(subCategoryAttributes.Value, Logger);

                    foreach (XmlNode item in subCategories.ChildNodes)
                    {
                        if (!(item.Name == "Item")) continue;

                        var itemId = -1;
                        var itemName = string.Empty;
                        var descriptionId = 0;
                        var prefabName = string.Empty;
                        var specialDisplayPrefab = string.Empty;
                        var tribe = TribeType.Unknown;
                        var rarity = ItemRarity.Unknown;
                        var memberOnly = false;

                        var currency = CurrencyType.Unknown;
                        var storeType = StoreType.Invalid;
                        var stockPriority = 0;
                        var regularPrice = 0;
                        var discountPrice = 0;
                        var sellPrice = 0;
                        var sellCount = 0;
                        var discountedFrom = new DateTime(0L);
                        var discountedTo = new DateTime(0L);

                        var actionType = ItemActionType.None;
                        var cooldownTime = 0f;
                        var delayUseDuration = 0;
                        var binding = ItemBinding.Unknown;

                        var level = -1;
                        var levelRequirement = -1;

                        var itemEffects = new List<ItemEffect>();

                        var uniqueInInventory = false;

                        var lootId = -1;
                        var recipeParentItemId = -1;

                        var productionStatus = ProductionStatus.Unknown;
                        var releaseDate = DateTime.UnixEpoch;

                        foreach (XmlAttribute itemAttributes in item.Attributes)
                            switch (itemAttributes.Name)
                            {
                                case "itemId":
                                    itemId = int.Parse(itemAttributes.Value);
                                    break;

                                case "itemName":
                                    itemName = itemAttributes.Value;
                                    break;
                                case "descriptionId":
                                    descriptionId = int.Parse(itemAttributes.Value);
                                    break;
                                case "prefabName":
                                    prefabName = itemAttributes.Value;
                                    break;
                                case "specialDisplayPrefab":
                                    specialDisplayPrefab = itemAttributes.Value;
                                    break;
                                case "tribe":
                                    tribe = tribe.GetEnumValue(itemAttributes.Value, Logger);
                                    break;
                                case "rarity":
                                    rarity = rarity.GetEnumValue(itemAttributes.Value, Logger);
                                    break;
                                case "memberOnly":
                                    memberOnly = memberOnly.GetBoolValue(itemAttributes.Value, Logger);
                                    break;

                                case "currency":
                                    currency = currency.GetEnumValue(itemAttributes.Value, Logger);
                                    break;
                                case "regularPrice":
                                    regularPrice = int.Parse(itemAttributes.Value);
                                    break;
                                case "sellPrice":
                                    sellPrice = int.Parse(itemAttributes.Value);
                                    break;
                                case "sellCount":
                                    sellCount = int.Parse(itemAttributes.Value);
                                    break;

                                case "actionType":
                                    actionType = actionType.GetEnumValue(itemAttributes.Value, Logger);
                                    break;
                                case "cooldownTime":
                                    cooldownTime = float.Parse(itemAttributes.Value);
                                    break;
                                case "delayUseDuration":
                                    delayUseDuration = int.Parse(itemAttributes.Value);
                                    break;
                                case "binding":
                                    binding = binding.GetEnumValue(itemAttributes.Value, Logger);
                                    break;

                                case "level":
                                    level = int.Parse(itemAttributes.Value);
                                    break;
                                case "levelRequirement":
                                    levelRequirement = int.Parse(itemAttributes.Value);
                                    break;

                                case "uniqueInInventory":
                                    uniqueInInventory = uniqueInInventory.GetBoolValue(itemAttributes.Value, Logger);
                                    break;

                                case "storeType":
                                    storeType = storeType.GetEnumValue(itemAttributes.Value, Logger);
                                    break;
                                case "discountedFrom":
                                    discountedFrom = discountedFrom.GetDateValue(itemAttributes.Value, Logger);
                                    break;
                                case "discountedTo":
                                    discountedTo = discountedTo.GetDateValue(itemAttributes.Value, Logger);
                                    break;
                                case "discountPrice":
                                    discountPrice = int.Parse(itemAttributes.Value);
                                    break;
                                case "stockPriority":
                                    stockPriority = int.Parse(itemAttributes.Value);
                                    break;

                                case "lootId":
                                    lootId = int.Parse(itemAttributes.Value);
                                    break;
                                case "recipeParentItemId":
                                    recipeParentItemId = int.Parse(itemAttributes.Value);
                                    break;

                                case "productionStatus":
                                    productionStatus = productionStatus.GetEnumValue(itemAttributes.Value, Logger);
                                    break;
                                case "releaseDate":
                                    releaseDate = releaseDate.GetDateValue(itemAttributes.Value, Logger);
                                    break;
                            }

                        foreach (XmlNode itemEffect in item.ChildNodes)
                        {
                            if (itemEffect.Name != "ItemEffects") continue;

                            foreach (XmlNode effect in itemEffect.ChildNodes)
                            {
                                if (effect.Name != "Effect") continue;

                                var type = ItemEffectType.Unknown;
                                var value = -1;
                                var duration = -1;

                                foreach (XmlAttribute effectAttributes in effect.Attributes)
                                    switch (effectAttributes.Name)
                                    {
                                        case "type":
                                            type = type.GetEnumValue(effectAttributes.Value, Logger);
                                            break;
                                        case "value":
                                            value = int.Parse(effectAttributes.Value);
                                            break;
                                        case "duration":
                                            duration = int.Parse(effectAttributes.Value);
                                            break;
                                    }
                                itemEffects.Add(new ItemEffect(type, value, duration));
                            }
                        }

                        if (!miscDict.LocalizationDict.TryGetValue(descriptionId, out var description))
                        {
                            Logger.LogError("Could not find description of id {DescId} for item {ItemName}", descriptionId, itemName);
                            continue;
                        }

                        Descriptions.TryAdd(descriptionId, description);

                        var nameId = miscDict.LocalizationDict.FirstOrDefault(x => x.Value == itemName);

                        if (string.IsNullOrEmpty(nameId.Value))
                        {
                            Logger.LogError("Could not find name for item {ItemName} in misc dictionary", itemName);
                            continue;
                        }

                        Descriptions.TryAdd(nameId.Key, nameId.Value);

                        if (!string.IsNullOrEmpty(prefabName))
                            Items.Add(new ItemDescription(itemId,
                                tribe, itemCategory, subCategory, actionType,
                                (int)rarity, currency, regularPrice, sellPrice, sellCount,
                                specialDisplayPrefab, itemName, description, prefabName,
                                cooldownTime, binding, level, levelRequirement, itemEffects, uniqueInInventory,
                                storeType, discountedFrom, discountedTo, discountPrice, stockPriority, lootId,
                                productionStatus, recipeParentItemId, releaseDate, memberOnly, delayUseDuration
                            ));
                    }
                }
            }
        }
    }

    public void FinalizeBundle()
    {
    }
}
