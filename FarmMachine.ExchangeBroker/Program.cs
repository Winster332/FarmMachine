using System;
using System.IO;
using Serilog;
using Topshelf;

namespace FarmMachine.ExchangeBroker
{
  class Program
  {
    static void Main(string[] args)
    {
      InitLog();

      HostFactory.Run(cfg =>
      {
        cfg.SetServiceName(FarmMachineExchangeBrokerService.Name);
        cfg.SetDescription(FarmMachineExchangeBrokerService.Description);
        cfg.SetDisplayName(FarmMachineExchangeBrokerService.Title);
        cfg.UseSerilog();
        cfg.Service<FarmMachineExchangeBrokerService>();
        cfg.RunAsLocalSystem();
        cfg.OnException(ex => Log.Error(ex.ToString()));
      });
    }

    private static void InitLog()
    {
      var pathToLog = "..\\Logs";
      if (!Directory.Exists(pathToLog))
      {
        Directory.CreateDirectory(pathToLog);
      }
      
      Log.Logger = new LoggerConfiguration()
        .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
        .WriteTo.File($"{pathToLog}\\FarmMachine.ExchangeBroker-.log", 
          outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}", 
          rollingInterval: RollingInterval.Day)
        .CreateLogger();
    }
  }
}