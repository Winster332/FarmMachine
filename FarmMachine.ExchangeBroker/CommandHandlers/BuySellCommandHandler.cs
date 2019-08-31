using System.Collections.Generic;
using System.Threading.Tasks;
using FarmMachine.Domain.Commands.Exchange;
using MassTransit;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FarmMachine.ExchangeBroker.CommandHandlers
{
  public class BuySellCommandHandler : IConsumer<BuyCurrency>, IConsumer<SellCurrency>
  {
    public IMongoCollection<BsonDocument> _protocol;
    public BuySellCommandHandler(IMongoDatabase database)
    {
      _protocol = database.GetCollection<BsonDocument>("protocol");
    }
    
    public async Task Consume(ConsumeContext<BuyCurrency> context)
    {
      await _protocol.InsertOneAsync(new BsonDocument(new Dictionary<string, object>
      {
        { "_id", context.Message.Id },
        { "amount", context.Message.Amount },
        { "bid", context.Message.Bid },
        { "timestamp", context.Message.Created },
        { "type", "buy" }
      }));
    }

    public async Task Consume(ConsumeContext<SellCurrency> context)
    {
      await _protocol.InsertOneAsync(new BsonDocument(new Dictionary<string, object>
      {
        { "_id", context.Message.Id },
        { "amount", context.Message.Amount },
        { "ask", context.Message.Ask },
        { "timestamp", context.Message.Created },
        { "type", "sell" }
      }));
    }
  }
}