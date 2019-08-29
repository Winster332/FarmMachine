using System;
using System.Collections.Generic;
using FarmMachine.MonitorStrategy.Core.Models;

namespace FarmMachine.MonitorStrategy.Core.Events
{
  public interface DetectedBacktestOrder
  {
    Guid Id { get; set; }
    DateTime Created { get; set; }
    OrderEventBacktest OrderEvent { get; set; }
  }
}