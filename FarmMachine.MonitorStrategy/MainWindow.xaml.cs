using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using CefSharp;
using CsQuery;
using FarmMachine.Domain.Commands.Exchange;
using FarmMachine.Domain.Models;
using FarmMachine.Domain.Services;
using FarmMachine.MonitorStrategy.Services;
using MassTransit;
using Serilog;

namespace FarmMachine.MonitorStrategy
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private LibraryLoader _libLoader;
    private OrderCacheValidator _orderCache;
    private TimeSchedulerService _timeScheduler;
    private MTBus _mtBus;
    
    private static void InitLog()
    {
      var pathToLog = "..\\Logs";
      if (!Directory.Exists(pathToLog))
      {
        Directory.CreateDirectory(pathToLog);
      }
      
      Log.Logger = new LoggerConfiguration()
        .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
        .WriteTo.File($"{pathToLog}\\FarmMachine.MonitorStrategy-.log", 
          outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}", 
          rollingInterval: RollingInterval.Day)
        .CreateLogger();
    }
    
    public MainWindow()
    {
      InitLog();
      
      Log.Information("Service starting...");
      
      CefSharp.CefSettings settings = new CefSharp.CefSettings();
      settings.CachePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\CEF"; 
      CefSharp.Cef.Initialize(settings);
      
      InitializeComponent();
      
      _libLoader = new LibraryLoader();
      _libLoader.Init();
      
      _mtBus = new MTBus();
      _orderCache = new OrderCacheValidator();
      _orderCache.DetectOrder += OrderCacheOnDetectOrder;
      
      Browser.FrameLoadEnd += WebBrowserFrameLoadEnded;
      this.Closing += OnClosing;
      
      _timeScheduler = new TimeSchedulerService(TimeSpan.FromMinutes(15));
      _timeScheduler.Work += TimeSchedulerOnWork;
      _timeScheduler.Start();
      
      Log.Information("Service started");
    }

    private void OrderCacheOnDetectOrder(object sender, OrderEventBacktest order)
    {
      if (order.EventType == OrderEventType.Buy)
      {
        Log.Information($"Buy order: {order}");
          
        
        _mtBus.GetBus().Publish<BuyCurrency>(new
        {
          Id = Guid.NewGuid(),
          Created = DateTime.Now,
          Amount = 0,
          Bid = order.Price
        });
      }
      else if (order.EventType == OrderEventType.Sell)
      {
        Log.Information($"Sell order: {order}");
        
        _mtBus.GetBus().Publish<SellCurrency>(new
        {
          Id = Guid.NewGuid(),
          Created = DateTime.Now,
          Amount = 0,
          Ask = order.Price
        });
      }
      else if (order.EventType == OrderEventType.Unknown)
      {
        Log.Information($"Unknown order: {order}");
      }
    }

    private void TimeSchedulerOnWork(object sender, EventArgs e)
    {
      Log.Warning("Browser start reloading from TimeScheduler...");
      Browser.Reload();
      Log.Warning("Browser stop reloaded from TimeScheduler");
    }

    private void WebBrowserFrameLoadEnded(object sender, FrameLoadEndEventArgs e)
    {
      if (e.IsMainFrame)
      {
        Browser.GetSourceAsync().ContinueWith(taskHtml =>
        {
          _libLoader.Execute(Browser);
          
          var html = taskHtml.Result;
          
          Browser.ExecuteScriptAsync("openLastBacktestOrderList();");
          
          Thread.Sleep(500);
          
          Browser.ExecuteScriptAsync("getButtonBacktestListOrders(2).click();");
          
          Thread.Sleep(500);
          
          Browser.ExecuteScriptAsync("backtestListOrderScrollToBottom();");
          
          Thread.Sleep(1000);
          
          Log.Information("Begin extract orders from backtest");
          var orders = ExtractOrders();
          Log.Information($"Orders extracted from backtest: {orders.Count}");
          
          Log.Information("Begin order validation");
          var validationResult = _orderCache.Push(orders);
          Log.Information($"End order validation: {validationResult}");

          if (validationResult.Status == ValidationStatus.Reload)
          {
            Thread.Sleep(1000);
            
            Log.Warning("Begin reload browser after failed validation");
            
            Browser.Reload();
          }
        });
      }
    }

    private List<BacktestOrderPair> ExtractOrders()
    {
      var htmlSource = Browser.GetSourceAsync().GetAwaiter().GetResult();
      var cq = CQ.Create(htmlSource);

      var items = cq.Find(".backtesting-content-wrapper").Find("tbody").ToList();
      var orderPairs = new List<BacktestOrderPair>();
      
      foreach (var tbody in items)
      {
        var orderPair = new BacktestOrderPair();
        
        var trs = tbody.Cq().Find("tr").ToList();
        var firstTr = trs.FirstOrDefault()?.ChildElements.Skip(2).ToList();
        var secondTr = trs.LastOrDefault()?.ChildElements.Skip(1).ToList();

        var ftype = firstTr[0].TextContent;
        var fdateTime = firstTr[1].TextContent;
        var fprice = firstTr[2].TextContent;

        orderPair.Left = OrderEventBacktest.Parse(ftype, fdateTime, fprice);
        
        if (!string.IsNullOrWhiteSpace(secondTr[1].TextContent))
        {
          var stype = secondTr[0].TextContent;
          var sdateTime = secondTr[1].TextContent;
          var sprice = secondTr[2].TextContent;
          
          orderPair.Right = OrderEventBacktest.Parse(stype, sdateTime, sprice);
        }
        else
        {
          orderPair.Right = null;
        }
        
        orderPairs.Add(orderPair);
      }
      
      orderPairs.RemoveAt(orderPairs.Count - 1);

      return orderPairs;
    }
    
    private void OnClosing(object sender, CancelEventArgs e)
    {
      _mtBus.GetBus().Stop();
      _timeScheduler.Stop();
    }
  }
}
