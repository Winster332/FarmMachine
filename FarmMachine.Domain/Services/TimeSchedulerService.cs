using System;
using System.Threading;

namespace FarmMachine.Domain.Services
{
  public class TimeSchedulerService
  {
    public event EventHandler Work;
    private Thread _thread;
    private bool _isWork;
    private DateTime _nextDateTimeBilling;
    private TimeSpan _interval;
    
    public TimeSchedulerService(TimeSpan interval)
    {
      _interval = interval;
      _nextDateTimeBilling = RoundUp(DateTime.Now, interval);
      _isWork = false;
      _thread = new Thread(DoWork);
    }

    private DateTime RoundUp(DateTime dt, TimeSpan d)
    {
      return new DateTime(((dt.Ticks + d.Ticks - 1) / d.Ticks) * d.Ticks);
    }

    private void DoWork()
    {
      while (_isWork)
      {
        var dateTimeNow = DateTime.Now;

        if (dateTimeNow >= _nextDateTimeBilling)
        {
          _nextDateTimeBilling = RoundUp(DateTime.Now, _interval);

          Work?.Invoke(this, null);
        }

        Thread.Sleep(TimeSpan.FromMinutes(1));
      }
    }

    public void Start()
    {
      _isWork = true;
      _thread.Start();
    }

    public void Stop()
    {
      _isWork = false;
      _thread.Abort();
    }
  }
}