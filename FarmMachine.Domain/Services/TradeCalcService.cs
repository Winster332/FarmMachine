namespace FarmMachine.Domain.Services
{
  public interface ITradeCalcService
  {
    decimal GetFee();
    decimal GetBuyAmount(decimal amount, decimal bidPrice);
  }

  public class TradeCalcService : ITradeCalcService
  {
    public const decimal Fee = 0.0025m;

    public decimal GetFee()
    {
      return Fee;
    }

    public decimal GetBuyAmount(decimal amount, decimal bidPrice)
    {
      var fullAmount = amount;

      var resultAmount = fullAmount / bidPrice;

      var resultWithFee = resultAmount * GetFee();
      var result = resultAmount - resultWithFee;

      return result;
    }
  }
}