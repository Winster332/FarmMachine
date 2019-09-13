using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FarmMachine.Domain.Models;
using MongoDB.Driver;
using Serilog;

namespace FarmMachine.Domain.Services
{
  public interface IReportGeneratorService
  {
    ReportResult Generate(DateTime from, DateTime to);
  }

  public class ReportGeneratorService : IReportGeneratorService
  {
    private IProtocolService _protocolService;
    
    public ReportGeneratorService(IProtocolService protocolService)
    {
      _protocolService = protocolService;
    }
    
    public ReportResult Generate(DateTime from, DateTime to)
    {
      try
      {
        var events = _protocolService.Get(from, to);
        var lines = new List<string>();

        foreach (var e in events)
        {
          var line = new List<string>();
          line.Add(e.Id.ToString());
          line.Add(e.Created.ToString());
          
          foreach (var keyValuePair in e.Fields)
          {
            var value = keyValuePair.Value;
            
            line.Add(value.ToString());
          }
            
          line.Add(e.Description);
        }

        var fileSource = string.Join(",", lines);

        using (var stream = new FileStream($"report {from.Day}.{from.Month}.{from.Year}-{to.Day}.{to.Month}.{to.Year}", FileMode.Create))
        {
          using (var writer = new StreamWriter(stream))
          {
            writer.WriteLine(fileSource);
          }
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex.ToString());
      }

      return null;
    }
  }
}