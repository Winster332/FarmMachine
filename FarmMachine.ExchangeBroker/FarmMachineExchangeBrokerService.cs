using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Autofac;
using Bittrex.Net;
using Bittrex.Net.Objects;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Logging;
using FarmMachine.ExchangeBroker.CommandHandlers;
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
      
      
      
      builder.RegisterInstance(Bus.Factory.CreateUsingRabbitMq(x =>
      {
        var host = x.Host(_settings.RabbitMQ.Host, h => { });

        x.UseDelayedExchangeMessageScheduler();

        x.ReceiveEndpoint(host, "farm_machine", cfg =>
        {
            cfg.Consumer(typeof(BuySellCommandHandler), f => new BuySellCommandHandler(_container.Resolve<IMongoDatabase>()));
//            cfg.Consumer(typeof(MailingQueryHandler), f => new MailingQueryHandler(database));

          cfg.UseConcurrencyLimit(_settings.RabbitMQ.ConcurrencyLimit);
        });

        x.UseConcurrencyLimit(_settings.RabbitMQ.ConcurrencyLimit);
      })).As<IBusControl>().SingleInstance();

      _container = builder.Build();
      
      _container.Resolve<IBittrexExchange>().Init();
      _container.Resolve<IBusControl>().Start();

      Log.Information($"Service {Name} v.{Version} started");
      
      return true;
    }

    public bool Stop(HostControl hostControl)
    {
      _container.Resolve<IBusControl>().Stop();
      _container.Resolve<IBittrexExchange>().Dispose();
      
      return true;
    }
  }
}