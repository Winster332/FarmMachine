using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace FarmMachine.Domain.Services
{
  public interface IProtocolService
  {
    Guid Write(IDictionary<string, object> fields, string description = null);
    Task<Guid> WriteAsync(IDictionary<string, object> fields, string description = null);
  }

  public class ProtocolService : IProtocolService
  {
    private IMongoCollection<ProtocolEvent> _protocols;

    public ProtocolService(IMongoDatabase db)
    {
      _protocols = db.GetCollection<ProtocolEvent>("protocol-events");
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

  public class ProtocolEvent
  {
    public Guid Id { get; set; }
    public DateTime Created { get; set; }
    public IDictionary<string, object> Fields { get; set; }
    public string Description { get; set; }
    public bool IsRemoved { get; set; }
  }
}