using System;
using System.Collections.Generic;
using System.Linq;
using FarmMachine.Domain.Commands.Exchange;
using FarmMachine.Domain.Models;
using Serilog;

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

    public override string ToString()
    {
      return $"Status [{Status}] Detected count [{DetectedCount}] OrderEvent.Type [{Order.EventType}] OrderEvent.DateTime [{Order.DateTime}] OrderEvent.Price [{Order.Price}]";
    }
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
      Log.Information("Push validation orders");
      
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
        Log.Information("Detected new order from backtest");
        
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
          
          Log.Information($"Order not equals by time. Need hour: [{nowHour}], fact: [{targetOrder.DateTime.Hour}]");

          listNew.Remove(targetOrder);
          foreach (var newOrder in listNew)
          {
            _cache.Remove(newOrder.DateTime);
          }
        }
      }
      else
      {
        Log.Information("Not found order from backtest");
        
        result.Status = ValidationStatus.NotFound;
      }

      Cleanup();

      return result;
    }

    private void PushNotification(OrderEventBacktest order)
    {
      if (order.EventType == OrderEventType.Buy)
      {
        Log.Information($"Buy order: {order}");
          
        _mtBus.GetBus().Publish<BuyCurrency>(new
        {
          Id = Guid.NewGuid(),
          Created = DateTime.Now,
          Amount = 0,
          Bid = order.Price
        });
      }
      else if (order.EventType == OrderEventType.Sell)
      {
        Log.Information($"Sell order: {order}");
        
        _mtBus.GetBus().Publish<SellCurrency>(new
        {
          Id = Guid.NewGuid(),
          Created = DateTime.Now,
          Amount = 0,
          Ask = order.Price
        });
      }
      else if (order.EventType == OrderEventType.Unknown)
      {
        Log.Information($"Unknown order: {order}");
      }
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