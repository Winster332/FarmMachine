using System;

namespace FarmMachine.Domain.Commands.Exchange
{
  public interface BuyCurrency
  {
    Guid Id { get; set; }
    DateTime Created { get; set; }
    
    decimal Amount { get; set; }
    decimal Bid { get; set; }
  }
}