namespace FarmMachine.MonitorStrategy.Core.Models
{
  public class BacktestOrderPair
  {
    public OrderEventBacktest Left { get; set; }
    public OrderEventBacktest Right { get; set; }
  }
}