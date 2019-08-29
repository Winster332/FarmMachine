using System;
using System.Collections.Generic;
using System.Linq;
using FarmMachine.MonitorStrategy.Core.Models;

namespace FarmMachine.MonitorStrategy.Core
{
  public class OrderCacheValidator
  {
    private Dictionary<DateTime, OrderEventBacktest> _cache { get; set; }

    public OrderCacheValidator()
    {
      _cache = new Dictionary<DateTime, OrderEventBacktest>();
    }

    public void Push(List<BacktestOrderPair> pairs)
    {
      var orders = pairs.Select(x => x.Right).Where(x => x != null).ToList();
      var listNew = new List<OrderEventBacktest>();

      foreach (var order in orders)
      {
        if (!_cache.ContainsKey(order.DateTime))
        {
          _cache.Add(order.DateTime, order);
          
          listNew.Add(order);
        }
      }

      if (listNew.Count != 0)
      {
        var targetOrder = listNew.LastOrDefault();
        
        Console.WriteLine("123");
      }

      Cleanup();
    }

    private void Cleanup()
    {
      var borderDateTime = DateTime.Now.AddMonths(-6);

      foreach (var orderEventBacktest in _cache)
      {
        var order = orderEventBacktest.Value;

        if (order.DateTime <= borderDateTime)
        {
          _cache.Remove(orderEventBacktest.Key);
        }
      }
    }
  }
}