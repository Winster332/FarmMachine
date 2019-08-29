using System;
using CsQuery.EquationParser.Implementation;
using GreenPipes;
using MassTransit;

namespace FarmMachine.MonitorStrategy.Core
{
  public class MTBus
  {
    private IBusControl _busControl;

    public MTBus()
    {
      _busControl = Bus.Factory.CreateUsingRabbitMq(x =>
      {
        var host = x.Host(new Uri("rabbitmq://127.0.0.1/cartrek_dev"), h => { });

        x.UseDelayedExchangeMessageScheduler();

        x.UseConcurrencyLimit(64);
      });

      _busControl.Start();
    }

    public IBusControl GetBus()
    {
      return _busControl;
    }
  }
}