using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FarmMachine.Domain.Models;
using MongoDB.Driver;

namespace FarmMachine.Domain.Services
{
  public interface IProtocolService
  {
    Guid Write(IDictionary<string, object> fields, string description = null);
    Task<Guid> WriteAsync(IDictionary<string, object> fields, string description = null);
    List<ProtocolEvent> Get(DateTime from, DateTime to);
  }

  public class ProtocolService : IProtocolService
  {
    private IMongoCollection<ProtocolEvent> _protocols;

    public ProtocolService(IMongoDatabase db)
    {
      _protocols = db.GetCollection<ProtocolEvent>("protocol-events");
    }

    public List<ProtocolEvent> Get(DateTime from, DateTime to)
    {
      var events = _protocols.Find(x => x.Created >= from && x.Created <= to).ToList();
      
      return events;
    }
    
    public Guid Write(IDictionary<string, object> fields, string description = null)
    {
      var e = new ProtocolEvent
      {
        Id = Guid.NewGuid(),
        Created = DateTime.Now,
        Fields = fields,
        Description = description,
        IsRemoved = false
      };
      
      _protocols.InsertOne(e);

      return e.Id;
    }

    public async Task<Guid> WriteAsync(IDictionary<string, object> fields, string description = null)
    {
      var e = new ProtocolEvent
      {
        Id = Guid.NewGuid(),
        Created = DateTime.Now,
        Fields = fields,
        Description = description,
        IsRemoved = false
      };
      
      await _protocols.InsertOneAsync(e);

      return e.Id;
    }
  }
}