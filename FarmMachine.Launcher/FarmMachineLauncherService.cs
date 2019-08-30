using System;
using System.IO;
using System.Reflection;
using FarmMachine.Launcher.Extensions;
using FarmMachine.Launcher.Services;
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
    public DotNetBuilderService BuilderService { get; set; }
    public DotNetLauncherService LauncherService { get; set; }
    
    public bool Start(HostControl hostControl)
    {
      Log.Information($"Service {Name} v.{Version} starting...");

      Log.Information($"Name: {Name}");
      Log.Information($"Title: {Title}");
      Log.Information($"Description: {Description}");
      Log.Information($"Version: {Version}");
      
      BuilderService = new DotNetBuilderService();
      LauncherService = new DotNetLauncherService(BuilderService);
      
      BuilderService.Load(new[]
      {
        @"FarmMachine.ExchangeBroker\bin\Debug\netcoreapp3.0\FarmMachine.ExchangeBroker.exe",
        @"FarmMachine.Logic\bin\Debug\FarmMachine.Logic.exe"
      });
      Log.Warning("Begin building projects");
      Console.WriteLine();
      foreach (var path in BuilderService.GetAllPaths())
      {
        Log.Warning($"\t+ {path}");
      }
      Console.WriteLine();
      BuilderService.Build();
      
      Log.Warning("Starting modules...");
      LauncherService.Start();
      Log.Warning("All modules started");
      
      return true;
    }

    public bool Stop(HostControl hostControl)
    {
      Log.Warning("Stopping modules");
      LauncherService.Start();
      return true;
    }
  }
}