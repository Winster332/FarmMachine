using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace FarmMachine.ExchangeBroker
{
  public class ExchangeSettings
  {
    public string BittrexKey { get; set; }
    public string BittrexSecret { get; set; }
    public MongoDbSettings Db { get; set; }
    public RabbitMqSettings RabbitMQ { get; set; }

    public void Load()
    {
      var fileSource = LoadFromFile();
      var json = JToken.Parse(fileSource);

      BittrexKey = json["bittrex"]["key"].ToObject<string>();
      BittrexSecret = json["bittrex"]["secret"].ToObject<string>();
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