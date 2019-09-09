using System;
using System.Collections.Generic;
using FarmMachine.Domain.Models;
using FarmMachine.Domain.Services;
using Xunit;

namespace FarmMachine.Tests.Tests
{
  public class OrderCacheValidatorTest : FarmMachineTest
  {
    [Fact]
    public void OrdersCahchIsEmptyTest()
    {
      var pairs = GetPairs();
      var countDetectedOrders = 0;
      var orderCache = new OrderCacheValidator();
      
      orderCache.DetectOrder += (sender, backtest) =>
      {
        countDetectedOrders++;
      };

      orderCache.Push(pairs);
      
      Assert.Equal(countDetectedOrders, 0);
    }
    
    [Fact]
    public void OrdersCahchIsFillTest()
    {
      var pairs = GetPairs();
      var countDetectedOrders = 0;
      var orderCache = new OrderCacheValidator();
      
      orderCache.DetectOrder += (sender, backtest) =>
      {
        countDetectedOrders++;
      };

      orderCache.Push(pairs);
      orderCache.Push(pairs);
      
      Assert.Equal(countDetectedOrders, 0);
    }
    
    [Fact]
    public void OrdersCacheOnDetectTimeTest()
    {
      var pairs = GetDetectTimePairs();
      var countDetectedOrders = 0;
      var orderCache = new OrderCacheValidator();
      
      orderCache.DetectOrder += (sender, backtest) =>
      {
        countDetectedOrders++;
      };

      orderCache.Push(pairs);
      orderCache.Push(pairs);
      
      var doublePairts = new List<BacktestOrderPair>();
      var now = DateTime.Now;
      
      // long => short
      doublePairts.Add(new BacktestOrderPair
      {
        Left = new OrderEventBacktest
        {
          EventType = OrderEventType.Buy,
          Price = 10262.28600000m,
          DateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0)
        },
        Right = new OrderEventBacktest
        {
          EventType = OrderEventType.Sell,
          Price = 10271.48700000m,
          DateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, 30, 0)
        }
      });
      orderCache.Push(pairs);
      
      Assert.Equal(countDetectedOrders, 1);
    }
    
    private List<BacktestOrderPair> GetDetectTimePairs()
    {
      var pairs = new List<BacktestOrderPair>();
      var now = DateTime.Now;
      
      // long => short
      pairs.Add(new BacktestOrderPair
      {
        Left = new OrderEventBacktest
        {
          EventType = OrderEventType.Buy,
          Price = 10262.28600000m,
          DateTime = new DateTime(now.Year, now.Month, now.Day, 5, 0, 0)
        },
        Right = new OrderEventBacktest
        {
          EventType = OrderEventType.Sell,
          Price = 10271.48700000m,
          DateTime = new DateTime(now.Year, now.Month, now.Day, 6, 0, 0)
        }
      });
      
      // short => long
      pairs.Add(new BacktestOrderPair
      {
        Left = new OrderEventBacktest
        {
          EventType = OrderEventType.Sell,
          Price = 10271.48700000m,
          DateTime = new DateTime(now.Year, now.Month, now.Day, 6, 0, 0)
        },
        Right = new OrderEventBacktest
        {
          EventType = OrderEventType.Buy,
          Price = 10252.00000000m,
          DateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0)
        }
      });

      // long => short
      pairs.Add(new BacktestOrderPair
      {
        Left = new OrderEventBacktest
        {
          EventType = OrderEventType.Buy,
          Price = 10252.00000000m,
          DateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0)
        }
      });
      
      return pairs;
    }

    private List<BacktestOrderPair> GetPairs()
    {
      var pairs = new List<BacktestOrderPair>();
      var now = DateTime.Now;
      
      // long => short
      pairs.Add(new BacktestOrderPair
      {
        Left = new OrderEventBacktest
        {
          EventType = OrderEventType.Buy,
          Price = 10262.28600000m,
          DateTime = new DateTime(now.Year, now.Month, now.Day, 5, 0, 0)
        },
        Right = new OrderEventBacktest
        {
          EventType = OrderEventType.Sell,
          Price = 10271.48700000m,
          DateTime = new DateTime(now.Year, now.Month, now.Day, 6, 0, 0)
        }
      });
      
      // short => long
      pairs.Add(new BacktestOrderPair
      {
        Left = new OrderEventBacktest
        {
          EventType = OrderEventType.Sell,
          Price = 10271.48700000m,
          DateTime = new DateTime(now.Year, now.Month, now.Day, 6, 0, 0)
        },
        Right = new OrderEventBacktest
        {
          EventType = OrderEventType.Buy,
          Price = 10252.00000000m,
          DateTime = new DateTime(now.Year, now.Month, now.Day, 7, 0, 0)
        }
      });

      // long => short
      pairs.Add(new BacktestOrderPair
      {
        Left = new OrderEventBacktest
        {
          EventType = OrderEventType.Buy,
          Price = 10252.00000000m,
          DateTime = new DateTime(now.Year, now.Month, now.Day, 7, 0, 0)
        }
      });
      
      return pairs;
    }
  }
}