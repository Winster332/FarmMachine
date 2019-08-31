using System;
using System.Threading;
using System.Timers;
using CefSharp.Wpf;
using Timer = System.Timers.Timer;

namespace FarmMachine.MonitorStrategy.Core
{
  public enum TimeInterval
  {
    M15, M30, 
    H1, H2, H3, H4,
    D1, W1, Month
  }
  
  public class TimeSchedulerService
  {
    private Thread _thread;
    private ChromiumWebBrowser _browser;
    private bool _isWork;
    private TimeInterval _interval;
    
    public TimeSchedulerService(ChromiumWebBrowser browser, TimeInterval interval)
    {
      _browser = browser;
      _isWork = false;
      _interval = interval;
      
//      Interval = TimeSpan.FromMinutes(15);
      
      _thread = new Thread(DoWork);
    }

    private void DoWork()
    {
      while (_isWork)
      {
        Thread.Sleep(1);
      }
    }

    private DateTime? GetDateTime(TimeInterval interval)
    {
      var result = DateTime.Now;
      
      if (interval == TimeInterval.D1)
      {
        
      }

      return null;
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