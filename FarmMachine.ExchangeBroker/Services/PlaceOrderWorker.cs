using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FarmMachine.Domain.Models;
using MongoDB.Bson.Serialization.Serializers;

namespace FarmMachine.ExchangeBroker.Services
{
  public class PlaceOrderWorker
  {
    private Thread _thread;
    private bool _isWork;
    private TimeSpan _timeoutOnChange { get; set; }
    private IPlaceOrderControlService _placeOrderControlService;
    public event EventHandler<MetaOrder> RefreshOnSell;
    public event EventHandler<MetaOrder> RefreshOnBuy;

    public PlaceOrderWorker(IPlaceOrderControlService placeOrderControlService)
    {
      _placeOrderControlService = placeOrderControlService;
      _isWork = false;
      _thread = new Thread(DoWork);
      _timeoutOnChange = TimeSpan.FromSeconds(10);
    }

    public void Start()
    {
      _isWork = true;
      _thread.Start();
    }

    private void DoWork()
    {
      while (_isWork)
      {
        var orders = _placeOrderControlService.ReadAllOrders();
        var removedOrders = new List<MetaOrder>();
        
        if (orders.Count != 0)
        {
          for (var i = 0; i < orders.Count; i++)
          {
            var order = orders[i];
            var elapsed = DateTime.Now - order.Created;

            if (elapsed.TotalSeconds >= 10)
            {
              var isOpen = _placeOrderControlService.GetExchange().IsOpenOrder(order.OrderId).GetAwaiter().GetResult();

              if (isOpen)
              {
                if (order.OrderType == OrderEventType.Buy)
                {
                  RefreshOnBuy?.Invoke(this, order);
                  
                  removedOrders.Add(order);
                }
                else if (order.OrderType == OrderEventType.Sell)
                {
                  RefreshOnSell?.Invoke(this, order);
                  
                  removedOrders.Add(order);
                }
              }
              else
              {
                removedOrders.Add(order);
              }
            }
          }

          if (removedOrders.Count != 0)
          {
            for (var i = 0; i < removedOrders.Count; i++)
            {
              var removeOrder = removedOrders[i];
              
              _placeOrderControlService.RemoveOrder(removeOrder.OrderId);
            }
          }
        }
        
        Thread.Sleep(_timeoutOnChange);
      }
    }

    public void Stop()
    {
      _isWork = false;
    }
  }
}