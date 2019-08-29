using System;

namespace FarmMachine.MonitorStrategy.Core.Models
{
  public class OrderEventBacktest
  {
    public OrderEventType EventType { get; set; }
    public DateTime DateTime { get; set; }
    public decimal Price { get; set; }

    public OrderEventBacktest()
    {
      EventType = OrderEventType.Unknown;
      Price = decimal.Zero;
    }

    public override string ToString()
    {
      return $"[{DateTime}] {EventType} {Price}";
    }

    public static OrderEventBacktest Parse(string eventType, string dateTime, string price)
    {
      var orderEvent = new OrderEventBacktest();

      if (eventType == "SELL")
      {
        orderEvent.EventType = OrderEventType.Sell;
      }
      else if (eventType == "BUY")
      {
        orderEvent.EventType = OrderEventType.Buy;
      }

      if (System.DateTime.TryParse(dateTime, out System.DateTime dateTimeResult))
      {
        orderEvent.DateTime = dateTimeResult;
      }

      if (decimal.TryParse(price, out decimal priceResult))
      {
        orderEvent.Price = priceResult;
      }

      return orderEvent;
    }
  }
}