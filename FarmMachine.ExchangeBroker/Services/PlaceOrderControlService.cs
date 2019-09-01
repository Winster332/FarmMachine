using System;
using System.Collections.Generic;
using FarmMachine.Domain.Models;
using FarmMachine.ExchangeBroker.Exchanges;

namespace FarmMachine.ExchangeBroker.Services
{
  public interface IPlaceOrderControlService
  {
    void AddOrder(MetaOrder metaOrder);
    List<MetaOrder> ReadAllOrders();
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

    public void AddOrder(MetaOrder metaOrder)
    {
      _orders.Add(metaOrder);
    }

    public List<MetaOrder> ReadAllOrders()
    {
      return _orders;
    }
  }
}