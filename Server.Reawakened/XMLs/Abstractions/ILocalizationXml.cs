using System.Xml;

namespace Server.Reawakened.XMLs.Abstractions;

public interface ILocalizationXml : IBundledXml
{
    string LocalizationName { get; }

    void EditLocalization(XmlDocument xml);
    void ReadLocalization(string xml);
}
