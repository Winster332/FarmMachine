using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace FarmMachine.Domain
{
  public class ExchangeSettings
  {
    public TelegramSettings Telegram { get; set; }
    public BittrexSettings Bittrex { get; set; }
    public MongoDbSettings Db { get; set; }
    public RabbitMqSettings RabbitMQ { get; set; }

    public void Load()
    {
      var fileSource = LoadFromFile();
      var json = JToken.Parse(fileSource);

      Bittrex = new BittrexSettings
      {
        Key = json["bittrex"]["key"].ToObject<string>(),
        Secret = json["bittrex"]["secret"].ToObject<string>(),
        Market = json["bittrex"]["market"].ToObject<string>(),
        
        RiskManager = new BittrexRiskManagerSettings
        {
          BaseCurrency = json["bittrex"]["riskManager"]["baseCurrency"].ToObject<string>(),
          Type = json["bittrex"]["riskManager"]["type"].ToObject<string>() == "percent" ? 
            RiskManagerBalanceType.Percent : RiskManagerBalanceType.Fixed,
          Percent = json["bittrex"]["riskManager"]["percent"].ToObject<int>(),
          Amount = json["bittrex"]["riskManager"]["amount"].ToObject<decimal>(),
          BalanceMinLimit = json["bittrex"]["riskManager"]["balanceMinLimit"].ToObject<decimal>()
        }
      };
      Db = new MongoDbSettings
      {
        DbConnectoin = json["database"]["connectionString"].ToObject<string>(),
        DbName = json["database"]["dataBaseName"].ToObject<string>()
      };
      RabbitMQ = new RabbitMqSettings
      {
        Host = new Uri(json["rabbitMq"]["host"].ToObject<string>()),
        ConcurrencyLimit = json["rabbitMq"]["concurrencyLimit"].ToObject<int>()
      };
      Telegram = new TelegramSettings
      {
        ApiKey = json["telegram"]["apiKey"].ToObject<int>(),
        ApiHash = json["telegram"]["apiHash"].ToObject<string>(),
        PhoneNumber = json["telegram"]["phoneNumber"].ToObject<string>()
      };
    }

    private string LoadFromFile()
    {
      var fileSource = string.Empty;
      
      using (var stream = new FileStream("appsettings.json", FileMode.Open))
      {
        using (var reader = new StreamReader(stream))
        {
          fileSource = reader.ReadToEnd();
        }
      }

      return fileSource;
    }

    public class TelegramSettings
    {
      public int ApiKey { get; set; }
      public string ApiHash { get; set; }
      public string PhoneNumber { get; set; }
    }

    public enum RiskManagerBalanceType
    {
      Fixed,
      Percent
    }

    public class BittrexRiskManagerSettings
    {
      public string BaseCurrency { get; set; }
      public RiskManagerBalanceType Type { get; set; }
      public decimal Amount { get; set; }
      public int Percent { get; set; }
      public decimal BalanceMinLimit { get; set; }
    }

    public class BittrexSettings
    {
      public string Key { get; set; }
      public string Secret { get; set; }
      public string Market { get; set; }
      
      public BittrexRiskManagerSettings RiskManager { get; set; }
    }

    public class RabbitMqSettings
    {
      public Uri Host { get; set; }
      public int ConcurrencyLimit { get; set; }
    }

    public class MongoDbSettings
    {
      public string DbConnectoin { get; set; }
      public string DbName { get; set; }
    }
  }
}