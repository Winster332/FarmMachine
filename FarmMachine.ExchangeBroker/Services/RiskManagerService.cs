using System.Linq;
using System.Threading.Tasks;
using FarmMachine.ExchangeBroker.Exchanges;

namespace FarmMachine.ExchangeBroker.Services
{
  public interface IRiskManagerService
  {
    Task<decimal> GetActualSellAmount();
    Task<decimal> GetActualBuyAmount();
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

    public async Task<decimal> GetActualSellAmount()
    {
      return await _exchange.GetBalance(CurrencySecond);
    }
    
    public async Task<decimal> GetActualBuyAmount()
    {
      return await _exchange.GetBalance(CurrencyFirst);
    }
  }
}