using System.Threading.Tasks;
using FarmMachine.Domain.Commands.Exchange;
using MassTransit;
using Serilog;

namespace FarmMachine.ExchangeBroker.CommandHandlers
{
  public class BuySellFaultHandler : IConsumer<Fault<BuyCurrency>>, IConsumer<Fault<SellCurrency>>
  {
    public Task Consume(ConsumeContext<Fault<BuyCurrency>> context)
    {
      return Task.Factory.StartNew(() =>
      {
        Log.Error($"FAULT [BuyCurrency]: {context.Message.Message}");
      });
    }

    public Task Consume(ConsumeContext<Fault<SellCurrency>> context)
    {
      return Task.Factory.StartNew(() =>
      {
        Log.Error($"FAULT [SellCurrency]: {context.Message.Message}");
      });
    }
  }
}