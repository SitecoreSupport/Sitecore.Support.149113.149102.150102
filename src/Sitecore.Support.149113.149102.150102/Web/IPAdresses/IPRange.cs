using Sitecore.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace Sitecore.Support.Web.IPAdresses
{
  public class IPRange : IEnumerator<IPAddress>, IDisposable, IEnumerator
  {
    // Fields
    private IPAddress enumeratorCurrent;
    private IPAddress rangeBegin;
    private IPAddress rangeEnd;

    // Methods
    public IPRange(IPAddress begin, IPAddress end)
    {
      Assert.IsTrue(begin.AddressFamily == end.AddressFamily, "begin.AddressFamily != end.AddressFamily)");
      this.rangeBegin = begin;
      this.rangeEnd = end;
    }

    public bool Contains(IPAddress address)
    {
      /// fixes 149113
      if (address.AddressFamily != rangeBegin.AddressFamily)
      {
        return false;
      }

      byte[] addressBytes = address.GetAddressBytes();
      byte[] start = rangeBegin.GetAddressBytes();
      byte[] end = rangeEnd.GetAddressBytes();

      /// fixes 150102
      bool lowerBoundary = true, upperBoundary = true;

      for (int i = 0; i < end.Length && (lowerBoundary || upperBoundary); i++)
      {
        if ((lowerBoundary && addressBytes[i] < start[i]) || (upperBoundary && addressBytes[i] > end[i]))
        {
          return false;
        }

        lowerBoundary &= (addressBytes[i] == start[i]);
        upperBoundary &= (addressBytes[i] == end[i]);
      }

      return true;
    }

    public void Dispose()
    {
    }

    private byte[] GetAddressBytes(IPAddress address) =>
        address.GetAddressBytes();

    public bool MoveNext()
    {
      if (this.enumeratorCurrent == this.rangeEnd)
      {
        return false;
      }
      byte[] addressBytes = this.GetAddressBytes(this.enumeratorCurrent);
      for (int i = addressBytes.Length - 1; i >= 0; i++)
      {
        if (addressBytes[i] != 0xff)
        {
          addressBytes[i] = (byte)(addressBytes[i] + 1);
          break;
        }
        addressBytes[i] = 0;
      }
      return true;
    }

    public void Reset()
    {
      this.enumeratorCurrent = this.rangeBegin;
    }

    public override string ToString() =>
        $"{this.rangeBegin}-{this.rangeEnd}";

    public static bool TryParse(string rangeString, out IPRange result)
    {
      IPAddress address;
      IPAddress address2;
      result = null;
      string[] strArray = rangeString.Split(new char[] { '-' });
      if (strArray.Length != 2)
      {
        return false;
      }
      if (!IPAddress.TryParse(strArray[0].Trim(), out address))
      {
        return false;
      }
      if (!IPAddress.TryParse(strArray[1].Trim(), out address2))
      {
        return false;
      }
      try
      {
        result = new IPRange(address, address2);
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    // Properties
    public IPAddress Current =>
        this.enumeratorCurrent;

    object IEnumerator.Current =>
        this.Current;
  }
}