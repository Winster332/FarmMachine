using System;

namespace FarmMachine.Domain.Services
{
  public interface ITimeService
  {
    DateTime GetNow();
    DateTime GetUTCNow();
  }

  public class TimeService : ITimeService
  {
    public DateTime GetNow()
    {
      return DateTime.Now;
    }
    
    public DateTime GetUTCNow()
    {
      return DateTime.UtcNow;
    }
  }
}