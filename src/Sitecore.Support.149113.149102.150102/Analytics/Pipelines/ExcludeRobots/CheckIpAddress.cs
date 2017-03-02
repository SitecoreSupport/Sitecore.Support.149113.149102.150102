using Sitecore.Analytics.Configuration;
using Sitecore.Analytics.Pipelines.ExcludeRobots;
using Sitecore.Diagnostics;
using Sitecore.Support.Analytics.Configuration;
using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;

namespace Sitecore.Support.Analytics.Pipelines.ExcludeRobots
{
  public class CheckIpAddress : ExcludeRobotsProcessor
  {
    protected readonly MethodInfo getHttpContext;

    public CheckIpAddress()
    {
      var prop = typeof(ExcludeRobotsArgs).GetProperty("HttpContext", BindingFlags.Instance | BindingFlags.NonPublic);
      getHttpContext = prop.GetMethod;
    }

    protected HttpContextBase GetValue(ExcludeRobotsArgs args)
    {
      return getHttpContext.Invoke(args, null) as HttpContextBase;
    }

    public override void Process(ExcludeRobotsArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      var httpContext = this.GetValue(args);
      Assert.IsNotNull(httpContext, "HttpContext");
      Assert.IsNotNull(httpContext.Request, "Request");
      /// fix for 149102 (or old number is 379609)
      string ipFromHttpHeader = this.GetIpFromHttpHeader(httpContext);
      if (string.IsNullOrEmpty(ipFromHttpHeader) && (httpContext.Request.UserHostAddress != null))
      {
        ipFromHttpHeader = httpContext.Request.UserHostAddress;
      }
      args.IsInExcludeList = Robots.ExcludeList.ContainsIpAddress(ipFromHttpHeader);
    }

    private string GetIpFromHttpHeader(HttpContextBase httpContext)
    {
      string forwardedRequestHttpHeader = AnalyticsSettings.ForwardedRequestHttpHeader;
      if (!string.IsNullOrEmpty(forwardedRequestHttpHeader))
      {
        string str2 = httpContext.Request.Headers[forwardedRequestHttpHeader];
        if (!string.IsNullOrEmpty(str2))
        {
          string str3 = str2.Split(new char[] { ',' }).Last<string>().Trim();
          if (string.IsNullOrEmpty(str3))
          {
            Log.Warn($"Sitecore.Support.149113.149102 :: {forwardedRequestHttpHeader} header does not store a valid IP address ({str2})", this);
          }
          else
          {
            IPAddress address;
            try
            {
              address = IPAddress.Parse(str3);
            }
            catch (FormatException)
            {
              Log.Warn($"Sitecore.Support.149113.149102 :: {forwardedRequestHttpHeader} header does not store a valid IP address ({str2})", this);
              return string.Empty;
            }
            return address.ToString();
          }
        }
      }
      return string.Empty;
    }

  }
}