using System.Reflection;
using Autofac;
using FarmMachine.Domain.Extensions;
using FarmMachine.ExchangeBroker.CommandHandlers;
using FarmMachine.ExchangeBroker.Exchanges;
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
      
      Log.Information($"");
      Log.Information($"Settings: ");
      Log.Information($"BITTREX");
      Log.Information($"BITTREX KEY: {_settings.Bittrex.Key}");
      Log.Information($"BITTREX SECRET: {_settings.Bittrex.Secret}");
      Log.Information($"BITTREX MARKET: {_settings.Bittrex.Market}");
      
      Log.Information($"");
      Log.Information($"Risk manager");
      Log.Information($"Risk manager type: {_settings.Bittrex.RiskManager.Type}");
      Log.Information($"Risk manager balance min limit: {_settings.Bittrex.RiskManager.BalanceMinLimit}");
      Log.Information($"Risk manager percent: {_settings.Bittrex.RiskManager.Percent}");
      Log.Information($"Risk manager amount: {_settings.Bittrex.RiskManager.Amount}");
      Log.Information($"Risk manager currency: {_settings.Bittrex.RiskManager.BaseCurrency}");

      Log.Information($"");
      Log.Information($"MongoDB");
      Log.Information($"Db connection: {_settings.Db.DbConnectoin}");
      Log.Information($"Db name: {_settings.Db.DbName}");
      
      Log.Information($"");
      Log.Information($"RabbitMQ");
      Log.Information($"RabbitMQ host: {_settings.RabbitMQ.Host}");
      Log.Information($"RabbitMQ concurrency limit: {_settings.RabbitMQ.ConcurrencyLimit}");
      Log.Information($"");
      
      var mongoClient = new MongoClient(_settings.Db.DbConnectoin);
      var database = mongoClient.GetDatabase(_settings.Db.DbName);
      
      var exchange = new BittrexExchange(_settings);
      
      var builder = new ContainerBuilder();

      builder.RegisterInstance(database).As<IMongoDatabase>().SingleInstance();
      builder.RegisterInstance(_settings).SingleInstance();
      builder.RegisterInstance(exchange).As<IBittrexExchange>().SingleInstance();
      
      builder.RegisterInstance(Bus.Factory.CreateUsingRabbitMq(x =>
      {
        var host = x.Host(_settings.RabbitMQ.Host, h => { });

        x.UseDelayedExchangeMessageScheduler();

        x.ReceiveEndpoint(host, "farm_machine", cfg =>
        {
            cfg.Consumer(typeof(BuySellCommandHandler), f => new BuySellCommandHandler(database, exchange));
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