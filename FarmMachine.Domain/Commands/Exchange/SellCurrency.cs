using System;

namespace FarmMachine.Domain.Commands.Exchange
{
  public interface SellCurrency
  {
    Guid Id { get; set; }
    DateTime Created { get; set; }
    
    decimal Amount { get; set; }
    decimal Ask { get; set; }
  }
}