namespace FarmMachine.Domain.Models
{
  public class ValidationResult
  {
    public ValidationStatus Status { get; set; }
    public int DetectedCount { get; set; }
    public OrderEventBacktest Order { get; set; }

    public override string ToString()
    {
      return $"Status [{Status}] Detected count [{DetectedCount}] OrderEvent.Type [{Order.EventType}] OrderEvent.DateTime [{Order.DateTime}] OrderEvent.Price [{Order.Price}]";
    }
  }
}