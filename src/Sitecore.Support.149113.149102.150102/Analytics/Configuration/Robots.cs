using Sitecore.Configuration;

namespace Sitecore.Support.Analytics.Configuration
{
  public static class Robots
  {
    // Fields
    private static ExcludeList excludeList;
    private static object excludeListSync = new object();

    // Properties
    public static bool AutoDetect =>
        Settings.GetBoolSetting("Analytics.AutoDetectBots", true);

    public static ExcludeList ExcludeList
    {
      get
      {
        if (excludeList == null)
        {
          lock (excludeListSync)
          {
            if (excludeList == null)
            {
              excludeList = ExcludeList.GeExcludeList();
            }
          }
        }
        return excludeList;
      }
      internal set
      {
        excludeList = value;
      }
    }

    public static bool IgnoreRobots =>
        Settings.GetBoolSetting("Analytics.Robots.IgnoreRobots", true);

    public static int SessionTimeout =>
        Settings.GetIntSetting("Analytics.Robots.SessionTimeout", 1);
  }


}