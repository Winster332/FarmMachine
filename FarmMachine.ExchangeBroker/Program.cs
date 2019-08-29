using System;
using Serilog;
using Topshelf;

namespace FarmMachine.ExchangeBroker
{
  class Program
  {
    static void Main(string[] args)
    {
      Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.File("FarmMachine.ExchangeBroker.log")
        .CreateLogger();

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
  }
}