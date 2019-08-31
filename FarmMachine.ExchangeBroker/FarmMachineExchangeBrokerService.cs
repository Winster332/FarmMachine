using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Autofac;
using Bittrex.Net;
using Bittrex.Net.Objects;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Logging;
using FarmMachine.ExchangeBroker.Exchanges;
using FarmMachine.ExchangeBroker.Extensions;
using GreenPipes;
using MassTransit;
using MongoDB.Driver;
using Topshelf;
using Log = Serilog.Log;

namespace FarmMachine.ExchangeBroker
{
  public class FarmMachineExchangeBrokerService : ServiceControl
  {
    public static string Name => Assembly.GetEntryAssembly().GetName().Name;
    public static string Title => Assembly.GetEntryAssembly().GetTitle();
    public static string Description => Assembly.GetEntryAssembly().GetDescription();
    public static string Version => Assembly.GetEntryAssembly().GetVersion();
    private IBusControl _busControl;
    private ExchangeSettings _settings;
    private IContainer _container;
    
    public bool Start(HostControl hostControl)
    {
      Log.Information($"Service {Name} v.{Version} starting...");

      Log.Information($"Name: {Name}");
      Log.Information($"Title: {Title}");
      Log.Information($"Description: {Description}");
      Log.Information($"Version: {Version}");
      
      _settings = new ExchangeSettings();
      _settings.Load();

      var builder = new ContainerBuilder();

      var mongoClient = new MongoClient(_settings.Db.DbConnectoin);
      builder.RegisterInstance(mongoClient.GetDatabase(_settings.Db.DbConnectoin)).As<IMongoDatabase>().SingleInstance();
      builder.RegisterInstance(_settings).SingleInstance();
      builder.RegisterType<BittrexExhange>().As<IBittrexExchange>().SingleInstance();
      
      _container = builder.Build();
      
      _container.Resolve<IBittrexExchange>().Init();
      
      _busControl = Bus.Factory.CreateUsingRabbitMq(x =>
      {
        var host = x.Host(new Uri("rabbitmq://127.0.0.1/cartrek_dev"), h => { });

        x.UseDelayedExchangeMessageScheduler();

        x.ReceiveEndpoint(host, "farm_machine", cfg =>
        {
//            cfg.Consumer(typeof(DetectedBacktestOrderEventHandler), f => new DetectedBacktestOrderEventHandler());
//            cfg.Consumer(typeof(MailingQueryHandler), f => new MailingQueryHandler(database));

          cfg.UseConcurrencyLimit(64);
        });

        x.UseConcurrencyLimit(64);
      });

      _busControl.Start();

      Log.Information($"Service {Name} v.{Version} started");
      
      return true;
    }

    public bool Stop(HostControl hostControl)
    {
      _busControl?.Stop();
      
      return true;
    }
  }
}