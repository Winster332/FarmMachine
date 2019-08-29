using System;
using FarmMachine.Domain.Models;

namespace FarmMachine.Domain.Events
{
  public interface DetectedBacktestOrder
  {
    Guid Id { get; set; }
    DateTime Created { get; set; }
    OrderEventBacktest OrderEvent { get; set; }
  }
}