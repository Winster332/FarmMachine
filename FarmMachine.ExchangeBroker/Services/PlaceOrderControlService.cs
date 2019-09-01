using System;
using System.Collections.Generic;
using System.Linq;
using FarmMachine.Domain.Models;
using FarmMachine.ExchangeBroker.Exchanges;

namespace FarmMachine.ExchangeBroker.Services
{
  public interface IPlaceOrderControlService
  {
    IBittrexExchange GetExchange();
    void AddOrder(MetaOrder metaOrder);
    List<MetaOrder> ReadAllOrders();
    void RemoveOrder(Guid Id);
  }

  public class PlaceOrderControlService : IPlaceOrderControlService
  {
    private IBittrexExchange _exchange;
    private List<MetaOrder> _orders;
    
    public PlaceOrderControlService(IBittrexExchange exchange)
    {
      _exchange = exchange;
      _orders = new List<MetaOrder>();
    }

    public IBittrexExchange GetExchange()
    {
      return _exchange;
    }

    public void AddOrder(MetaOrder metaOrder)
    {
      _orders.Add(metaOrder);
    }

    public List<MetaOrder> ReadAllOrders()
    {
      return _orders;
    }

    public void RemoveOrder(Guid Id)
    {
      var order = _orders.FirstOrDefault(x => x.OrderId == Id);
      
      if (order != null)
      {
        _orders.Remove(order);
      }
    }
  }
}