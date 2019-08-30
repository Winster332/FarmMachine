using System.Collections.Generic;
using System.Diagnostics;

namespace FarmMachine.Launcher.Services
{
  public class DotNetLauncherService
  {
    private DotNetBuilderService _builder;
    private Stack<Process> _stackProcesses;
    
    public DotNetLauncherService(DotNetBuilderService builder)
    {
      _builder = builder;
      _stackProcesses = new Stack<Process>();
    }

    public void Start()
    {
      var paths = _builder.GetAllPaths();

      foreach (var path in paths)
      {
        var process = Process.Start(path);
        _stackProcesses.Push(process);
      }
    }

    public void Stop()
    {
      var length = _stackProcesses.Count;

      for (var i = 0; i < length; i++)
      {
        var process = _stackProcesses.Pop();
        process.Kill();
      }
    }
  }
}