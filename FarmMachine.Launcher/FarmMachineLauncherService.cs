using System.Reflection;
using FarmMachine.Launcher.Extensions;
using Serilog;
using Topshelf;

namespace FarmMachine.Launcher
{
  public class FarmMachineLauncherService : ServiceControl
  {
    public static string Name => Assembly.GetEntryAssembly().GetName().Name;
    public static string Title => Assembly.GetEntryAssembly().GetTitle();
    public static string Description => Assembly.GetEntryAssembly().GetDescription();
    public static string Version => Assembly.GetEntryAssembly().GetVersion();
    
    public bool Start(HostControl hostControl)
    {
      Log.Information($"Service {Name} v.{Version} starting...");

      Log.Information($"Name: {Name}");
      Log.Information($"Title: {Title}");
      Log.Information($"Description: {Description}");
      Log.Information($"Version: {Version}");
      
      return true;
    }

    public bool Stop(HostControl hostControl)
    {
      return true;
    }
  }
}