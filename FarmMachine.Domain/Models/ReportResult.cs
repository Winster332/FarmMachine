using System;

namespace FarmMachine.Domain.Models
{
  public class ReportResult
  {
    public Guid Id { get; set; }
    public bool Success { get; set; }
    public string Error { get; set; }
  }
}