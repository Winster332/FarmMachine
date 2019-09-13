using System;
using System.Collections.Generic;

namespace FarmMachine.Domain.Models
{
  public class ProtocolEvent
  {
    public Guid Id { get; set; }
    public DateTime Created { get; set; }
    public IDictionary<string, object> Fields { get; set; }
    public string Description { get; set; }
    public bool IsRemoved { get; set; }
  }
}