using A2m.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Server.Base.Core.Extensions;
using Server.Reawakened.XMLs.Abstractions;
using Server.Reawakened.XMLs.BundlesInternal;
using Server.Reawakened.XMLs.Enums;
using Server.Reawakened.XMLs.Extensions;
using System.Reflection;
using System.Xml;

namespace Server.Reawakened.XMLs.Bundles;

public class ItemCatalog
{
    public string BundleName => "ItemCatalog";
    public string LocalizationName => "ItemCatalogDict_en-US";
    public BundlePriority Priority => BundlePriority.High;

    public Microsoft.Extensions.Logging.ILogger Logger { get; set; }
    public IServiceProvider Services { get; set; }

    private Dictionary<string, int> _itemNameDict;
    private Dictionary<ItemCategory, XmlNode> _itemCategories;
    private Dictionary<ItemCategory, Dictionary<ItemSubCategory, XmlNode>> _itemSubCategories;

    public Dictionary<int, ItemDescription> Items;

    public ItemCatalog()
    {
    }

    public void InitializeVariables()
    {
    }

    public void EditLocalization(XmlDocument xml)
    {
    }
    public void ReadLocalization(string xml)
    {
    }

    public void EditDescription(XmlDocument xml)
    {
    }

    public void ReadDescription(string xml)
    {
        //ReadDescriptionXml(xml);
    }

    public void FinalizeBundle()
    {
        //var field = typeof(GameGlobals).GetField("_itemHandler",
                    //BindingFlags.Static |
                   // BindingFlags.NonPublic);

        //field.SetValue(null, this);

        //Items = (Dictionary<int, ItemDescription>)this.GetField<ItemHandler>("_itemDescriptionCache");
    }
}
