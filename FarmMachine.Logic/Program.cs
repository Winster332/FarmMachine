using Serilog;
using Topshelf;

namespace FarmMachine.Logic
{
  internal class Program
  {
    public static void Main(string[] args)
    {
      Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.File("FarmMachine.Logic.log")
        .CreateLogger();

      HostFactory.Run(cfg =>
      {
        cfg.SetServiceName(FarmMachineLogicService.Name);
        cfg.SetDescription(FarmMachineLogicService.Description);
        cfg.SetDisplayName(FarmMachineLogicService.Title);
        cfg.UseSerilog();
        cfg.Service<FarmMachineLogicService>();
        cfg.RunAsLocalSystem();
        cfg.OnException(ex => Log.Error(ex.ToString()));
      });
    }
  }
}