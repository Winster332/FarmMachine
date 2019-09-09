using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FarmMachine.Domain.Commands.Exchange;
using FarmMachine.Domain.Models;
using FarmMachine.Domain.Services;
using FarmMachine.ExchangeBroker.Exchanges;
using MassTransit;
using MongoDB.Bson;
using MongoDB.Driver;
using Serilog;

namespace FarmMachine.ExchangeBroker.CommandHandlers
{
  public class BuySellCommandHandler : IConsumer<BuyCurrency>, IConsumer<SellCurrency>
  {
    public IMongoCollection<BsonDocument> _protocol;
    private IBittrexExchange _exchange;
    
    public BuySellCommandHandler(IMongoDatabase database, IBittrexExchange exchange)
    {
      _exchange = exchange;
      _protocol = database.GetCollection<BsonDocument>("protocol");
    }
    
    public async Task Consume(ConsumeContext<BuyCurrency> context)
    {
      // Указываем количество сколько нужно закупиться BTC
//      var amount = 0.001m;//await _exchange.RiskManager.GetActualBuyAmount();
      var amountInCurrency = await _exchange.RiskManager.GetActualBuyAmount();
      var rate = await _exchange.GetActualBuyPrice();
      var converter = new TradeCalcService();
      var amount = decimal.Round(converter.GetBuyAmount(amountInCurrency, rate), 8);
      var balanceMinLimit = _exchange.RiskManager.GetBalanceMinLimit();
      
      Log.Information($"GET[BUY] => USD amount [{amount} / {amountInCurrency}] rate [{rate}]");

      if (amountInCurrency <= balanceMinLimit)
      {
        Log.Warning($"Balance equal {amountInCurrency}. Risk manager stopping FarmMachine.ExchangeBroker");
        
        return;
      }

      try
      {
        await _protocol.InsertOneAsync(new BsonDocument(new Dictionary<string, object>
        {
          {"_id", context.Message.Id},
          {"amount", amount},
          {"bid", context.Message.Bid},
          {"rate", rate},
          {"timestamp", context.Message.Created},
          {"type", "buy"}
        }));
      }
      catch (Exception ex)
      {
        Log.Warning($"Invalid write to db: {ex}");
      }

//      await _exchange.PlaceOrderOnBuy(amount, rate);
    }

    public async Task Consume(ConsumeContext<SellCurrency> context)
    {
      // Количество сколько нужно продать BTC
      // Обернуть инсерты в базу в блоки ексепшена
//      var amountInCurrency = 0.00050000m;//await _exchange.RiskManager.GetActualSellAmount();
      var amountInCurrency = await _exchange.RiskManager.GetActualSellAmount();
      var rate = await _exchange.GetActualSellPrice();
      var converter = new TradeCalcService();
      var amount = decimal.Round(converter.GetSellAmount(amountInCurrency, rate), 8);
      var balanceMinLimit = _exchange.RiskManager.GetBalanceMinLimit();
      
      Log.Information($"GET[SELL] => USD amount [{amount} / {amountInCurrency}] rate [{rate}]");

      if (amount <= balanceMinLimit)
      {
        Log.Warning($"Balance equal {amountInCurrency}. Risk manager stopping FarmMachine.ExchangeBroker");

        return;
      }
      
      try
      {
        await _protocol.InsertOneAsync(new BsonDocument(new Dictionary<string, object>
        {
          {"_id", context.Message.Id},
          {"amount", context.Message.Amount},
          {"ask", context.Message.Ask},
          {"rate", rate},
          {"timestamp", context.Message.Created},
          {"type", "sell"}
        }));
      }
      catch (Exception ex)
      {
        Log.Warning($"Invalid write to db: {ex}");
      }

//      await _exchange.PlaceOrderOnSell(amountInCurrency, rate);
    }
  }
}