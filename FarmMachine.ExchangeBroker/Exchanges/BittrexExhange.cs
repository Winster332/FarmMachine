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
  public interface IBittrexExchange
  {
    void Init();
  }

  public class BittrexExhange : IBittrexExchange
  {
    private ExchangeSettings _settings;
    public BittrexExhange(ExchangeSettings settings)
    {
      _settings = settings;
    }

    public void Init()
    {
//      var settings = _container.Resolve<ExchangeSettings>();
      BittrexClient.SetDefaultOptions(new BittrexClientOptions()
      {
        ApiCredentials = new ApiCredentials(_settings.BittrexKey, _settings.BittrexSecret),
        LogVerbosity = LogVerbosity.Info,
        LogWriters = new List<TextWriter>() { Console.Out }
      });
    }
  }
}