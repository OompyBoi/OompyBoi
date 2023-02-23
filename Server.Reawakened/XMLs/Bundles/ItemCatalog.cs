using A2m.Server;
using Server.Base.Core.Extensions;
using Server.Reawakened.XMLs.Abstractions;
using Server.Reawakened.XMLs.Extensions;
using System.Xml;

namespace Server.Reawakened.XMLs.Bundles;

public class ItemCatalog : ItemHandler, ILocalizationXml
{
    public string BundleName => "ItemCatalog";
    public string LocalizationName => "ItemCatalogDict_en-US";

    public ItemCatalog() : base(null) {}

    public void InitializeVariables()
    {
        this.SetField<ItemHandler>("_isDisposed", false);
        this.SetField<ItemHandler>("_initDescDone", false);
        this.SetField<ItemHandler>("_initLocDone", false);

        this.SetField<ItemHandler>("_localizationDict", new Dictionary<int, string>());
        this.SetField<ItemHandler>("_itemDescriptionCache", new Dictionary<int, ItemDescription>());
        this.SetField<ItemHandler>("_pendingRequests", new Dictionary<int, ItemDescriptionRequest>());
    }

    public void EditXml(XmlDocument xml) => AddCategoryPet(xml);

    public void ReadLocalization(string xml) => ReadLocalizationXml(xml);

    public void EditDescription(XmlDocument xml)
    {
    }

    public void EditLocalization(XmlDocument xml) => AddLocalizationText(xml);
    public void ReadLocalization(string xml) => ReadLocalizationXml(xml);

    public void FinalizeBundle() {}

    private static void AddLocalizationText(XmlDocument xml)
    {
        var root = xml.SelectSingleNode("/ItemCatalogDict");

        var enzoTextName = xml.CreateElement("text").AddAttribute(xml, "id", 10000);
        enzoTextName.InnerText = "Enzo";

        root.AppendChild(enzoTextName);
    }

    private static void AddCategoryPet(XmlDocument xml)
    {
        var root = xml.SelectSingleNode("/Catalog");

        var petCategory = xml.CreateElement("ItemCategory").AddAttribute(xml, "id", 16).AddAttribute(xml, "name", "Pet");
        var petSubCategory = xml.CreateElement("ItemSubcategory").AddAttribute(xml, "id", 47).AddAttribute(xml, "name", "Pet").AddAttribute(xml, "number_effects", 1);
        
        var enzoItem = xml.CreateElement("Item").AddAttribute(xml, "action_type", "Pet").AddAttribute(xml, "bind_type", "OnEquip")
            .AddAttribute(xml, "cooldown_time", "1.0").AddAttribute(xml, "currency", "Banana").AddAttribute(xml, "discounted_from", "None")
            .AddAttribute(xml, "discounted_to", "None").AddAttribute(xml, "global_level", 1).AddAttribute(xml, "id", 4000).AddAttribute(xml, "ingamename", 10000)
            .AddAttribute(xml, "item_level", 1).AddAttribute(xml, "name", "PF_PET_Rig02_Enzo01").AddAttribute(xml, "prefab", "PF_PET_Rig02_Enzo01")
            .AddAttribute(xml, "prefab_type", "Pet").AddAttribute(xml, "price", 0).AddAttribute(xml, "price_discount", -1)
            .AddAttribute(xml, "production_status", "Production").AddAttribute(xml, "rarity", "Common")
            .AddAttribute(xml, "sell_price", 0).AddAttribute(xml, "stock_priority", 1).AddAttribute(xml, "store_type", "Front Store")
            .AddAttribute(xml, "tribe", "Crossroads").AddAttribute(xml, "unique_in_inventory", "false");

        petSubCategory.AppendChild(enzoItem);
        petCategory.AppendChild(petSubCategory);

        root.AppendChild(petCategory);
    }
}
