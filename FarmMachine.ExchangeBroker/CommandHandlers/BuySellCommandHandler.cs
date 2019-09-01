using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FarmMachine.Domain.Commands.Exchange;
using FarmMachine.Domain.Models;
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
      var amount = 0.001m;//await _exchange.RiskManager.GetActualBuyAmount();
      var rate = await _exchange.GetActualBuyPrice();
      
      Log.Information($"GET[BUY] => USD amount [{amount}] by [{rate}]");

      if (amount <= 15)
      {
        Log.Warning($"Balance equal {amount}. Risk manager stopping FarmMachine.ExchangeBroker");
        
//        Environment.Exit(0);
      }

      await _protocol.InsertOneAsync(new BsonDocument(new Dictionary<string, object>
      {
        { "_id", context.Message.Id },
        { "amount", amount },
        { "bid", context.Message.Bid },
        { "timestamp", context.Message.Created },
        { "type", "buy" }
      }));

//      await _exchange.PlaceOrderOnBuy(amount, rate);
    }

    public async Task Consume(ConsumeContext<SellCurrency> context)
    {
      // Количество сколько нужно продать BTC
      // Обернуть инсерты в базу в блоки ексепшена
      var amount = 0.00050000m;//await _exchange.RiskManager.GetActualSellAmount();
      var rate = await _exchange.GetActualSellPrice();
      
      await _protocol.InsertOneAsync(new BsonDocument(new Dictionary<string, object>
      {
        { "_id", context.Message.Id },
        { "amount", context.Message.Amount },
        { "ask", context.Message.Ask },
        { "timestamp", context.Message.Created },
        { "type", "sell" }
      }));
      
      await _exchange.PlaceOrderOnSell(amount, rate);
    }
  }
}