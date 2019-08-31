using System;
using System.Collections.Generic;
using System.Linq;
using FarmMachine.Domain.Commands.Exchange;
using FarmMachine.Domain.Models;

namespace FarmMachine.MonitorStrategy.Core
{
  public enum ValidationStatus
  {
    Pushed,
    Reload,
    NotFound
  }

  public class ValidationResult
  {
    public ValidationStatus Status { get; set; }
    public int DetectedCount { get; set; }
    public OrderEventBacktest Order { get; set; }
  }

  public class OrderCacheValidator
  {
    private Dictionary<DateTime, OrderEventBacktest> _cache { get; set; }
    private MTBus _mtBus;

    public OrderCacheValidator(MTBus bus)
    {
      _mtBus = bus;
      _cache = new Dictionary<DateTime, OrderEventBacktest>();
    }

    public ValidationResult Push(List<BacktestOrderPair> pairs)
    {
      var result = new ValidationResult();
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

      result.DetectedCount = listNew.Count;
      result.Order = null;

      if (listNew.Count != 0)
      {
        var targetOrder = listNew.LastOrDefault();
        var nowHour = DateTime.Now.Hour;
        
        result.Order = targetOrder;

        if (targetOrder.DateTime.Hour == nowHour)
        {
          PushNotification(targetOrder);
          
          result.Status = ValidationStatus.Pushed;
        }
        else
        {
          result.Status = ValidationStatus.Reload;

          listNew.Remove(targetOrder);
          foreach (var newOrder in listNew)
          {
            _cache.Remove(newOrder.DateTime);
          }
        }
      }
      else
      {
        result.Status = ValidationStatus.NotFound;
      }

      Cleanup();

      return result;
    }

    private void PushNotification(OrderEventBacktest order)
    {
      if (order.EventType == OrderEventType.Buy)
      {
        _mtBus.GetBus().Publish<BuyCurrency>(new
        {
          Id = Guid.NewGuid(),
          Created = DateTime.Now,
          Amount = 18,
          Bid = order.Price
        });
      }
      else if (order.EventType == OrderEventType.Sell)
      {
        _mtBus.GetBus().Publish<SellCurrency>(new
        {
          Id = Guid.NewGuid(),
          Created = DateTime.Now,
          Amount = 18,
          Ask = order.Price
        });
      }
      else if (order.EventType == OrderEventType.Unknown)
      {
        Console.WriteLine("123");
      }

//      _mtBus.GetBus().Publish<DetectedBacktestOrder>(new
//      {
//        Id = Guid.NewGuid(),
//        Created = DateTime.Now,
//        OrderEvent = order,
//      }).GetAwaiter().GetResult();
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