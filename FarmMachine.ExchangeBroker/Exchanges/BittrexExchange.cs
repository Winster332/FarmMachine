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

namespace FarmMachine.ExchangeBroker.Exchanges
{
  public interface IBittrexExchange : IDisposable
  {
    void Init();

    Task<decimal> GetActualBuyPrice();
    Task<decimal> GetActualSellPrice();
  }

  public class BittrexExchange : IBittrexExchange
  {
    private ExchangeSettings _settings;
    private BittrexClient _httpClient;
    private BittrexSocketClient _socketClient;
    private string _marketName;
    public BittrexExchange(ExchangeSettings settings)
    {
      _settings = settings;
      _marketName = settings.Bittrex.Market;
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
    }

    public async Task<decimal> GetActualBuyPrice()
    {
      var orderBook = await _httpClient.GetOrderBookAsync(_marketName);
      var lastOrder = orderBook.Data.Buy.LastOrDefault();
      return lastOrder.Rate;
    }

    public async Task<decimal> GetActualSellPrice()
    {
      var orderBook = await _httpClient.GetOrderBookAsync(_marketName);
      var lastOrder = orderBook.Data.Sell.LastOrDefault();
      return lastOrder.Rate;
    }


    public void Dispose()
    {
      _socketClient?.UnsubscribeAll();
      
      _httpClient?.Dispose();
      _socketClient?.Dispose();
    }
  }
}