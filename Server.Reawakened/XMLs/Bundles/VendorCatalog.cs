using Server.Base.Core.Extensions;
using Server.Reawakened.XMLs.Abstractions;
using Server.Reawakened.XMLs.Extensions;
using System.Xml;

namespace Server.Reawakened.XMLs.Bundles;

public class VendorCatalog : VendorCatalogsXML, IBundledXml
{
    public string BundleName => "vendor_catalogs";

    public void InitializeVariables()
    {
        _rootXmlName = BundleName;
        _hasLocalizationDict = true;

        this.SetField<VendorCatalogsXML>("_levelUpCatalogs", new Dictionary<int, List<int>>());
        this.SetField<VendorCatalogsXML>("_vendorCatalogs", new Dictionary<int, List<int>>());
        this.SetField<VendorCatalogsXML>("_vendorCatalogIds", new Dictionary<string, int>());
        this.SetField<VendorCatalogsXML>("_vendorCatalogIdToVendorId", new Dictionary<int, int>());
        this.SetField<VendorCatalogsXML>("_levelUpCatalogs", new Dictionary<int, List<int>>());
        this.SetField<VendorCatalogsXML>("_cashShops", new Dictionary<string, CashShop>());
        this.SetField<VendorCatalogsXML>("_superPacks", new Dictionary<int, Dictionary<int, int>>());
        this.SetField<VendorCatalogsXML>("_loots", new Dictionary<int, List<LootData>>());
    }

    public void EditDescription(XmlDocument xml)
    {
        AddMogriVendor(xml);

        var items = xml.SelectNodes("/vendor_catalogs/superpacks/superpack/item");

        if (items == null)
            return;

        foreach (XmlNode aNode in items)
        {
            if (aNode.Attributes == null)
                continue;

            var idAttribute = aNode.Attributes["quantity"];

            if (idAttribute != null)
                continue;

            var quantity = xml.CreateAttribute("quantity");
            quantity.Value = "1";

            aNode.Attributes.Append(quantity);
        }
    }

    public void ReadDescription(string xml) => ReadDescriptionXml(xml);

    public void FinalizeBundle() {}

    private static void AddMogriVendor(XmlDocument xml)
    {
        var root = xml.SelectSingleNode("vendor_catalogs");

        var mogriVendor = xml.CreateElement("vendor").AddAttribute(xml, "catalogid", 116);

        var enzoItem = xml.CreateElement("item").AddAttribute(xml, "id", 4000);

        mogriVendor.AppendChild(enzoItem);
        root.AppendChild(mogriVendor);
    }
}
