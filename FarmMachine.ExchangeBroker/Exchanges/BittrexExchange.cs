using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Bittrex.Net;
using Bittrex.Net.Objects;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Logging;
using CryptoExchange.Net.OrderBook;
using FarmMachine.Domain.Models;
using FarmMachine.ExchangeBroker.Services;

namespace FarmMachine.ExchangeBroker.Exchanges
{
  public interface IBittrexExchange : IDisposable
  {
    void Init();

    IRiskManagerService RiskManager { get; set; }
    Task<decimal> GetActualBuyPrice();
    Task<decimal> GetActualSellPrice();
    Task<decimal> GetBalance(string currency);
    Task<Guid> PlaceOrderOnSell(decimal amount, decimal rate);
    Task<Guid> PlaceOrderOnBuy(decimal amount, decimal rate);
    Task CancelOrder(Guid orderId);
    PlaceOrderWorker GetPlaceWorker();
  }

  public class BittrexExchange : IBittrexExchange
  {
    private ExchangeSettings _settings;
    private BittrexClient _httpClient;
    private BittrexSocketClient _socketClient;
    private string _marketName;
    private PlaceOrderWorker _placeOrderWorker;
    public IRiskManagerService RiskManager { get; set; }
    public IPlaceOrderControlService PlaceOrderController { get; set; }
    
    public BittrexExchange(ExchangeSettings settings)
    {
      _settings = settings;
      _marketName = settings.Bittrex.Market;
      RiskManager = new RiskManagerService(this, settings.Bittrex.Market);
      PlaceOrderController = new PlaceOrderControlService(this);
      _placeOrderWorker = new PlaceOrderWorker(PlaceOrderController);
//      _placeOrderWorker.RefreshOnBuy += PlaceOrderWorkerOnRefreshOnBuy;
//      _placeOrderWorker.RefreshOnSell += PlaceOrderWorkerOnRefreshOnSell;
    }

    public void Init()
    {
      BittrexClient.SetDefaultOptions(new BittrexClientOptions()
      {
        ApiCredentials = new ApiCredentials(_settings.Bittrex.Key, _settings.Bittrex.Secret),
        LogVerbosity = LogVerbosity.Info,
        LogWriters = new List<TextWriter>() { Console.Out }
      });
      
      _httpClient = new BittrexClient();
      _socketClient = new BittrexSocketClient();
      
      _placeOrderWorker.Start();
    }

    public async Task<decimal> GetBalance(string currency)
    {
      var balance = await _httpClient.GetBalanceAsync(currency);
      return balance.Data.Available.Value;
    }

    public async Task<decimal> GetActualBuyPrice()
    {
      var orderBook = await _httpClient.GetOrderBookAsync(_marketName);
      var minOrder = GetMinOrderByRate(orderBook.Data.Sell);
      //// TODO: Надо проверять на наличие по количеству
      
      return minOrder.Rate;
    }
    
    private BittrexOrderBookEntry GetMinOrderByRate(List<BittrexOrderBookEntry> orders)
    {
      var result = default(BittrexOrderBookEntry);
      var maxRate = orders.FirstOrDefault()?.Rate;

      for (var i = 0; i < orders.Count; i++)
      {
        var order = orders[i];

        if (order.Rate <= maxRate)
        {
          result = order;
        }
      }

      return result;
    }

    private BittrexOrderBookEntry GetMaxOrderByRate(List<BittrexOrderBookEntry> orders)
    {
      var result = default(BittrexOrderBookEntry);
      var maxRate = orders.FirstOrDefault()?.Rate;

      for (var i = 0; i < orders.Count; i++)
      {
        var order = orders[i];

        if (order.Rate >= maxRate)
        {
          result = order;
        }
      }

      return result;
    }

    public async Task<decimal> GetActualSellPrice()
    {
      var orderBook = await _httpClient.GetOrderBookAsync(_marketName);
      var minOrder = GetMaxOrderByRate(orderBook.Data.Buy);
      
      return minOrder.Rate;
    }

    public async Task<Guid> PlaceOrderOnSell(decimal amount, decimal rate)
    {
//      var result = await _httpClient.PlaceOrderAsync(OrderSide.Sell, _marketName, amount, rate);

//      return result.Data.Uuid;
      var id = Guid.NewGuid();
      PlaceOrderController.AddOrder(new MetaOrder
      {
        OrderId = id,
        Amount = amount,
        Rate = rate,
        Created = DateTime.Now,
        OrderType = OrderEventType.Sell
      });
      return id;
    }

    public async Task<Guid> PlaceOrderOnBuy(decimal amount, decimal rate)
    {
      var id = Guid.NewGuid();
      PlaceOrderController.AddOrder(new MetaOrder
      {
        OrderId = id,
        Amount = amount,
        Rate = rate,
        Created = DateTime.Now,
        OrderType = OrderEventType.Buy
      });
//      var result = await _httpClient.PlaceOrderAsync(OrderSide.Buy, _marketName, amount, rate);

//      return result.Data.Uuid;
      return id;
    }

    public async Task CancelOrder(Guid orderId)
    {
      var response = await _httpClient.CancelOrderAsync(orderId);
    }

    public PlaceOrderWorker GetPlaceWorker()
    {
      return _placeOrderWorker;
    }

    public void Dispose()
    {
      _placeOrderWorker.Stop();
      _socketClient?.UnsubscribeAll();
      
      _httpClient?.Dispose();
      _socketClient?.Dispose();
    }
  }
}