using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace FarmMachine.Launcher.Services
{
  public class DotNetBuilderService
  {
    private List<string> _paths;
    
    public DotNetBuilderService()
    {
      _paths = new List<string>();
    }

    public void Load(string[] pathToProjects)
    {
      foreach (var localPath in pathToProjects)
      {
        var path = $"..\\..\\..\\..\\{localPath}";
        _paths.Add(path);
      }
    }

    public void Build()
    {
      foreach (var path in _paths)
      {
        var solutionPath = string.Join("\\", path.Split("\\").SkipLast(4));
        var process = Process.Start("dotnet", $"build {solutionPath}");

        var isRunningProcess = true;

        while (isRunningProcess)
        {
          try
          {
            var p = Process.GetProcessById(process.Id);
            var pNaem = p.ProcessName;

            Thread.Sleep(1000);
          }
          catch (InvalidOperationException ex)
          {
            isRunningProcess = false;
          }
        }
      }
    }

    public string[] GetAllPaths()
    {
      return _paths.ToArray();
    }
  }
}