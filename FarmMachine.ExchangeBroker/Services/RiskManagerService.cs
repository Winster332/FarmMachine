using System.Linq;
using System.Threading.Tasks;
using FarmMachine.ExchangeBroker.Exchanges;

namespace FarmMachine.ExchangeBroker.Services
{
  public interface IRiskManagerService
  {
    Task<decimal> GetActualSellAmount();
    Task<decimal> GetActualBuyAmount();
    decimal GetBalanceMinLimit();
  }

  public class RiskManagerService : IRiskManagerService
  {
    private IBittrexExchange _exchange;
    public string CurrencyFirst { get; set; }
    public string CurrencySecond { get; set; }
    private decimal _balanceMinLimit { get; set; }
    
    public RiskManagerService(IBittrexExchange exchange, string market, decimal balanceMinLimit)
    {
      _exchange = exchange;
      
      var pair = market.Split("-")
        .Select(x => x.Replace(" ", ""))
        .ToList();

      CurrencyFirst = pair.FirstOrDefault();
      CurrencySecond = pair.LastOrDefault();
      _balanceMinLimit = balanceMinLimit;
    }

    public async Task<decimal> GetActualSellAmount()
    {
      return await _exchange.GetBalance(CurrencySecond);
    }
    
    public async Task<decimal> GetActualBuyAmount()
    {
      return await _exchange.GetBalance(CurrencyFirst);
    }

    public decimal GetBalanceMinLimit()
    {
      return _balanceMinLimit;
    }
  }
}