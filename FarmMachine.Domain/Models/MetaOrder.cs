using System;

namespace FarmMachine.Domain.Models
{
  public class MetaOrder
  {
    public string OrderType { get; set; }
    public DateTime Created { get; set; }
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public decimal Rate { get; set; }

    public override string ToString()
    {
      return $"Type [{OrderType}] Created [{Created}] OrderId [{OrderId}] Amount [{Amount}] Rate [{Rate}]";
    }
  }
}