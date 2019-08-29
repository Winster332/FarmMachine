using System;
using System.Threading.Tasks;
using FarmMachine.Domain.Events;
using MassTransit;

namespace FarmMachine.Logic.EventHandlers
{
  public class DetectedBacktestOrderEventHandler : IConsumer<DetectedBacktestOrder>
  {
    public async Task Consume(ConsumeContext<DetectedBacktestOrder> context)
    {
      Console.WriteLine("123");
      var x = 10;
    }
  }
}