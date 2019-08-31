using System.Linq;
using FarmMachine.ExchangeBroker.Exchanges;

namespace FarmMachine.ExchangeBroker.Services
{
  public interface IRiskManagerService
  {
    decimal GetActualAmount();
  }

  public class RiskManagerService : IRiskManagerService
  {
    private IBittrexExchange _exchange;
    public string CurrencyFirst { get; set; }
    public string CurrencySecond { get; set; }
    public RiskManagerService(IBittrexExchange exchange, string market)
    {
      _exchange = exchange;
      
      var pair = market.Split("-")
        .Select(x => x.Replace(" ", ""))
        .ToList();

      CurrencyFirst = pair.FirstOrDefault();
      CurrencySecond = pair.LastOrDefault();
    }

    public decimal GetActualAmount()
    {
      return 0;
    }
  }
}