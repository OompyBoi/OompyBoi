using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Server.Reawakened.XMLs.Extensions;

public static class XmlExtensions
{
    public static XmlNode AddAttribute(this XmlNode node, XmlDocument doc, string name, object value)
    {
        var attr = doc.CreateAttribute(name);
        attr.Value = value.ToString();
        node.Attributes.Append(attr);
        return node;
    }
}
