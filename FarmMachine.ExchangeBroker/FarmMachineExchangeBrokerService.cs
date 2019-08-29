using System;
using System.Reflection;
using FarmMachine.ExchangeBroker.Extensions;
using GreenPipes;
using MassTransit;
using Serilog;
using Topshelf;

namespace FarmMachine.ExchangeBroker
{
  public class FarmMachineExchangeBrokerService : ServiceControl
  {
    public static string Name => Assembly.GetEntryAssembly().GetName().Name;
    public static string Title => Assembly.GetEntryAssembly().GetTitle();
    public static string Description => Assembly.GetEntryAssembly().GetDescription();
    public static string Version => Assembly.GetEntryAssembly().GetVersion();
    private IBusControl _busControl;
    
    public bool Start(HostControl hostControl)
    {
      Log.Information($"Service {Name} v.{Version} starting...");

      Log.Information($"Name: {Name}");
      Log.Information($"Title: {Title}");
      Log.Information($"Description: {Description}");
      Log.Information($"Version: {Version}");
      
      _busControl = Bus.Factory.CreateUsingRabbitMq(x =>
      {
        var host = x.Host(new Uri("rabbitmq://127.0.0.1/cartrek_dev"), h => { });

        x.UseDelayedExchangeMessageScheduler();

        x.ReceiveEndpoint(host, "farm_machine", cfg =>
        {
//            cfg.Consumer(typeof(DetectedBacktestOrderEventHandler), f => new DetectedBacktestOrderEventHandler());
//            cfg.Consumer(typeof(MailingQueryHandler), f => new MailingQueryHandler(database));

          cfg.UseConcurrencyLimit(64);
        });

        x.UseConcurrencyLimit(64);
      });

      _busControl.Start();

      Log.Information($"Service {Name} v.{Version} started");
      
      return true;
    }

    public bool Stop(HostControl hostControl)
    {
      _busControl?.Stop();
      
      return true;
    }
  }
}