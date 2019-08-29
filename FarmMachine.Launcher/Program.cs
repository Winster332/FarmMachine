using System;
using Serilog;
using Topshelf;

namespace FarmMachine.Launcher
{
  class Program
  {
    static void Main(string[] args)
    {
      Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.File("FarmMachine.Launcher.log")
        .CreateLogger();

      HostFactory.Run(cfg =>
      {
        cfg.SetServiceName(FarmMachineLauncherService.Name);
        cfg.SetDescription(FarmMachineLauncherService.Description);
        cfg.SetDisplayName(FarmMachineLauncherService.Title);
        cfg.UseSerilog();
        cfg.Service<FarmMachineLauncherService>();
        cfg.RunAsLocalSystem();
        cfg.OnException(ex => Log.Error(ex.ToString()));
      });
    }
  }
}