using System;
using System.Threading.Tasks;
using FarmMachine.Domain.Commands.Exchange;
using FarmMachine.Domain.Events;
using FarmMachine.Domain.Models;
using MassTransit;
using Serilog;

namespace FarmMachine.Logic.EventHandlers
{
  public class DetectedBacktestOrderEventHandler : IConsumer<DetectedBacktestOrder>
  {
    public async Task Consume(ConsumeContext<DetectedBacktestOrder> context)
    {
      if (context.Message.OrderEvent.EventType == OrderEventType.Buy)
      {
        await context.Publish<BuyCurrency>(new
        {
          Id = Guid.NewGuid(),
          Created = context.Message.Created,
          Amount = 18,
          Bid = context.Message.OrderEvent.Price
        });
      }
      else if (context.Message.OrderEvent.EventType == OrderEventType.Sell)
      {
        await context.Publish<SellCurrency>(new
        {
          Id = Guid.NewGuid(),
          Created = context.Message.Created,
          Amount = 18,
          Ask = context.Message.OrderEvent.Price
        });
      }
      else if (context.Message.OrderEvent.EventType == OrderEventType.Unknown)
      {
        Log.Information("Get unknown message");
      }
    }
  }
}