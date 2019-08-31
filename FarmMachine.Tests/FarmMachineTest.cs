using System;
using GreenPipes;
using MassTransit;

namespace FarmMachine.Tests
{
  public class FarmMachineTest
  {
    public IBusControl BusControl { get; set; }
    public FarmMachineTest()
    {
      BusControl = Bus.Factory.CreateUsingRabbitMq(x =>
      {
        var host = x.Host(new Uri("rabbitmq://127.0.0.1/cartrek_dev"), h => { });

        x.UseDelayedExchangeMessageScheduler();

        x.UseConcurrencyLimit(64);
      });
    }
  }
}