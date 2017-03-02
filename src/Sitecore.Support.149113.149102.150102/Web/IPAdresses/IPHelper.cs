using Sitecore.Diagnostics;
using System.Xml;

namespace Sitecore.Support.Web.IPAdresses
{
  public class IPHelper
  {
    public static IPList GetIPList(XmlNode node)
    {
      IPList list;
      Assert.IsNotNull(node, "node");
      IPList.TryParse(node.InnerText, out list);
      return list;
    }
  }
}