using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CefSharp;
using FarmMachine.MonitorStrategy.Core;

namespace FarmMachine.MonitorStrategy
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private LibraryLoader _libLoader;
    public MainWindow()
    {
      CefSharp.CefSettings settings = new CefSharp.CefSettings();
      settings.CachePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\CEF"; 
      CefSharp.Cef.Initialize(settings);
      
      InitializeComponent();
      
      _libLoader = new LibraryLoader();
      _libLoader.Init();
      
      Browser.FrameLoadEnd += WebBrowserFrameLoadEnded;
    }
    
    private void WebBrowserFrameLoadEnded(object sender, FrameLoadEndEventArgs e)
    {
      if (e.IsMainFrame)
      {
        Browser.GetSourceAsync().ContinueWith(taskHtml =>
        {
          _libLoader.Execute(Browser);
          
          var html = taskHtml.Result;
        });
      }
    }
  }
}
