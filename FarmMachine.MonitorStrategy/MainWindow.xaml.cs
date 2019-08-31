using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using CefSharp;
using CsQuery;
using FarmMachine.Domain.Models;
using FarmMachine.Domain.Services;
using FarmMachine.MonitorStrategy.Core;
using MassTransit;

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
    
    public MainWindow()
    {
      CefSharp.CefSettings settings = new CefSharp.CefSettings();
      settings.CachePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\CEF"; 
      CefSharp.Cef.Initialize(settings);
      
      InitializeComponent();
      
      _libLoader = new LibraryLoader();
      _libLoader.Init();
      
      _mtBus = new MTBus();
      _orderCache = new OrderCacheValidator(_mtBus);
      
      Browser.FrameLoadEnd += WebBrowserFrameLoadEnded;
      this.Closing += OnClosing;
      
      _timeScheduler = new TimeSchedulerService(TimeSpan.FromMinutes(15));
      _timeScheduler.Work += TimeSchedulerOnWork;
      _timeScheduler.Start();
    }

    private void TimeSchedulerOnWork(object sender, EventArgs e)
    {
      Browser.Reload();
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
          
          Thread.Sleep(500);
          
          var orders = ExtractOrders();
          var validationResult = _orderCache.Push(orders);

          if (validationResult.Status == ValidationStatus.Reload)
          {
            Thread.Sleep(1000);
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
