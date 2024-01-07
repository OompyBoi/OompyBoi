using A2m.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Server.Base.Core.Extensions;
using Server.Reawakened.XMLs.Abstractions;
using Server.Reawakened.XMLs.Bundles;
using Server.Reawakened.XMLs.Enums;
using Server.Reawakened.XMLs.Extensions;
using System.Reflection;
using System.Xml;

namespace Server.Reawakened.XMLs.BundlesInternal;

public class ItemCatalogInt : ItemHandler, IBundledXml, ILocalizationXml
{
    public string BundleName => "ItemCatalogInt";
    public string LocalizationName => "ItemCatalogIntDict_en-US";
    public BundlePriority Priority => BundlePriority.High;

    public Microsoft.Extensions.Logging.ILogger Logger { get; set; }
    public IServiceProvider Services { get; set; }

    private Dictionary<string, int> _itemNameDict;
    private Dictionary<ItemCategory, XmlNode> _itemCategories;
    private Dictionary<ItemCategory, Dictionary<ItemSubCategory, XmlNode>> _itemSubCategories;

    public Dictionary<int, ItemDescription> Items;
    public Dictionary<int, string> Descriptions;

    public ItemCatalogInt() : base(null)
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
            ReadLocalizationXml(xml.WriteToString());

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
            }
        }
    }

    public void ReadLocalization(string xml) =>
           ReadLocalizationXml(xml.ToString());

    public void EditDescription(XmlDocument xml)
    {
        _itemCategories.Clear();
        _itemSubCategories.Clear();

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
                    }
                }
            }
            var smallestItemId = 0;
        }
    }

    public void ReadDescription(string xml)
    {
        ReadDescriptionXml(xml);
    }

    public void FinalizeBundle()
    {
        var field = typeof(GameGlobals).GetField("_itemHandler",
        BindingFlags.Static |
        BindingFlags.NonPublic);

        field.SetValue(null, this);

        Items = (Dictionary<int, ItemDescription>)this.GetField<ItemHandler>("_itemDescriptionCache");
    }
}
