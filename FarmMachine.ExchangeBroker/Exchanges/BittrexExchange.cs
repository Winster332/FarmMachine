using System;
using System.Collections.Generic;
using System.IO;
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
  }

  public class BittrexExchange : IBittrexExchange
  {
    private ExchangeSettings _settings;
    private BittrexClient _httpClient;
    private BittrexSocketClient _socketClient;
    public BittrexExchange(ExchangeSettings settings)
    {
      _settings = settings;
    }

    public void Init()
    {
      BittrexClient.SetDefaultOptions(new BittrexClientOptions()
      {
        ApiCredentials = new ApiCredentials(_settings.BittrexKey, _settings.BittrexSecret),
        LogVerbosity = LogVerbosity.Info,
        LogWriters = new List<TextWriter>() { Console.Out }
      });
      
      _httpClient = new BittrexClient();
      _socketClient = new BittrexSocketClient();
    }

    public async void GetOrderBook()
    {
      var orderBook = await _httpClient.GetOrderBookAsync("USD-BTC");
    }

    public void Dispose()
    {
      _socketClient?.UnsubscribeAll();
      
      _httpClient?.Dispose();
      _socketClient?.Dispose();
    }
  }
}