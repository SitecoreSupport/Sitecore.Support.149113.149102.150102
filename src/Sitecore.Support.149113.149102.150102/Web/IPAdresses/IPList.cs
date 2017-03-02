using Sitecore.Diagnostics;
using Sitecore.Web.IPAddresses;
using System;
using System.Collections.Generic;
using System.Net;

namespace Sitecore.Support.Web.IPAdresses
{
  public class IPList
  {
    // Fields
    private HashSet<IPAddress> addresses = new HashSet<IPAddress>();
    private HashSet<Sitecore.Support.Web.IPAdresses.IPRange> ranges = new HashSet<Sitecore.Support.Web.IPAdresses.IPRange>();
    private HashSet<IPSubnet> subnets = new HashSet<IPSubnet>();

    // Methods
    public void Add(Sitecore.Support.Web.IPAdresses.IPRange range)
    {
      this.ranges.Add(range);
    }

    public void Add(IPSubnet subnet)
    {
      this.subnets.Add(subnet);
    }

    public void Add(IPAddress address)
    {
      this.addresses.Add(address);
    }

    public bool Contains(IPAddress address)
    {
      if (this.addresses.Contains(address))
      {
        return true;
      }
      foreach (IPSubnet subnet in this.subnets)
      {
        if (subnet.Contains(address))
        {
          return true;
        }
      }
      foreach (Sitecore.Support.Web.IPAdresses.IPRange range in this.ranges)
      {
        if (range.Contains(address))
        {
          return true;
        }
      }
      return false;
    }

    public static bool TryParse(string list, out IPList result)
    {
      result = null;
      if (string.IsNullOrEmpty(list))
      {
        return false;
      }
      string[] strArray = list.Split(new char[] { '\n' });
      if (strArray.Length == 0)
      {
        return false;
      }
      result = new IPList();
      foreach (string str in strArray)
      {
        string str2 = str.Trim();
        if (!string.IsNullOrEmpty(str2) && !str2.StartsWith("#", StringComparison.InvariantCulture))
        {
          IPSubnet subnet;
          if (!IPSubnet.TryParse(str2, out subnet))
          {
            Sitecore.Support.Web.IPAdresses.IPRange range;
            if (!Sitecore.Support.Web.IPAdresses.IPRange.TryParse(str2, out range))
            {
              IPAddress address;
              if (!IPAddress.TryParse(str2, out address))
              {
                Log.Warn(string.Format("Failed to initialize IPList. String {0} is not looks like valid value for IPAddress, IPRange or IPSubnet", new object[] { str2 }), new object());
                result = null;
                return false;
              }
              result.Add(address);
            }
            else
            {
              result.Add(range);
            }
          }
          else
          {
            result.Add(subnet);
          }
        }
      }
      return true;
    }
  }
}