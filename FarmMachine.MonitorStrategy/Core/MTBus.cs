using System;
using CsQuery.EquationParser.Implementation;
using GreenPipes;
using MassTransit;
using Serilog;

namespace FarmMachine.MonitorStrategy.Core
{
  public class MTBus
  {
    private IBusControl _busControl;

    public MTBus()
    {
      var connectionString = "rabbitmq://127.0.0.1/cartrek_dev";
      
      Log.Information($"RabbitMQ connection: {connectionString}");
      
      _busControl = Bus.Factory.CreateUsingRabbitMq(x =>
      {
        var host = x.Host(new Uri(connectionString), h => { });

        x.UseDelayedExchangeMessageScheduler();

        x.UseConcurrencyLimit(64);
      });

      _busControl.Start();
      
      Log.Information($"RabbitMQ started");
    }

    public IBusControl GetBus()
    {
      return _busControl;
    }
  }
}