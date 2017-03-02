using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Support.Web.IPAdresses;
using System.Collections.Generic;
using System.Net;
using System.Xml;

namespace Sitecore.Support.Analytics.Configuration
{
  public class ExcludeList
  {
    // Fields
    private IPList ipaddresses;
    private HashSet<string> userAgents;

    // Methods
    public ExcludeList(IPList ipaddresses, HashSet<string> userAgents)
    {
      this.ipaddresses = ipaddresses;
      this.userAgents = userAgents;
    }

    public ExcludeList(IPList ipaddresses, IEnumerable<string> userAgents) : this(ipaddresses, new HashSet<string>(userAgents))
    {
    }

    public bool ContainsIpAddress(string addressString)
    {
      IPAddress address;
      Assert.ArgumentNotNull(addressString, "addressString");
      return (IPAddress.TryParse(addressString, out address) && this.ipaddresses.Contains(address));
    }

    public bool ContainsUserAgent(string userAgent)
    {
      Assert.ArgumentNotNull(userAgent, "userAgent");
      return this.userAgents.Contains(userAgent);
    }

    public static ExcludeList GeExcludeList()
    {
      IPList iPAddresses = GetIPAddresses();
      return new ExcludeList(iPAddresses, GetUserAgents());
    }

    private static IPList GetIPAddresses()
    {
      XmlNode configNode = Factory.GetConfigNode("analyticsExcludeRobots/excludedIPAddresses");
      if (configNode == null)
      {
        return new IPList();
      }
      return (IPHelper.GetIPList(configNode) ?? new IPList());
    }

    private static HashSet<string> GetUserAgents()
    {
      XmlNode configNode = Factory.GetConfigNode("analyticsExcludeRobots/excludedUserAgents");
      HashSet<string> set = new HashSet<string>();
      if (configNode != null)
      {
        foreach (string str in configNode.InnerText.Split(new char[] { '\n' }))
        {
          string item = str.Trim();
          if (((item != string.Empty) && !item.StartsWith("#")) && !set.Contains(item))
          {
            set.Add(item);
          }
        }
      }
      return set;
    }
  }
}