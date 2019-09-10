using TLSharp.Core;

namespace FarmMachine.Domain.Services
{
  public interface ITelegramIntegrations
  {
  }

  public class TelegramIntegrations : ITelegramIntegrations
  {
    public TelegramIntegrations(ExchangeSettings.TelegramSettings settings)
    {
      var store = new FileSessionStore();
      var client = new TelegramClient(settings.ApiKey, settings.ApiHash, store);
    }
  }
}