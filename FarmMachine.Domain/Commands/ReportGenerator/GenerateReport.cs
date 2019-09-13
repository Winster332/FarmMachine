using System;

namespace FarmMachine.Domain.Commands.ReportGenerator
{
  public interface GenerateReport
  {
    Guid Id { get; set; }
    DateTime From { get; set; }
    DateTime To { get; set; }
  }

  public interface ReportGenerated
  {
    Guid Id { get; set; }
    Guid ReportId { get; set; }
    string FileName { get; set; }
  }
}