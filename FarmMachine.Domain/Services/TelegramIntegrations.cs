using System;
using System.Threading.Tasks;
using TeleSharp.TL;
using TLSharp.Core;

namespace FarmMachine.Domain.Services
{
  public interface ITelegramIntegrations
  {
    Task Start(Func<string> callbackCodeAuth);
    Task SendInfo(string text);
    Task SendWarn(string text);
    Task SendError(Exception ex);
  }

  public class TelegramIntegrations : ITelegramIntegrations
  {
    private FileSessionStore _store;
    private TelegramClient _client;
    private ExchangeSettings.TelegramSettings _settings;
    private TLUser _user;
    public TelegramIntegrations(ExchangeSettings.TelegramSettings settings)
    {
      _settings = settings;
      _store = new FileSessionStore();
      _client = new TelegramClient(_settings.ApiKey, _settings.ApiHash, _store, "session");
    }

    public async Task Start(Func<string> callbackCodeAuth)
    {
      await _client.ConnectAsync();

      var hash = await _client.SendCodeRequestAsync(_settings.PhoneNumber);
      var code = callbackCodeAuth();

      _user = await _client.MakeAuthAsync(_settings.PhoneNumber, hash, code);
    }

    public async Task SendInfo(string text)
    {
      await _client.SendMessageAsync(new TLInputPeerSelf(), $"{GetFormatDateTime()} [INFO] {text}");
    }

    public async Task SendWarn(string text)
    {
      await _client.SendMessageAsync(new TLInputPeerSelf(), $"{GetFormatDateTime()} [WARN] {text}");
    }
    
    public async Task SendError(Exception ex)
    {
      await _client.SendMessageAsync(new TLInputPeerSelf(), $"{GetFormatDateTime()} [ERROR] {ex.ToString()}");
    }

    private string GetFormatDateTime()
    {
      var now = DateTime.Now;
      
      return $"[{now.Day}.{now.Month}.{now.Year} {now.Hour}:{now.Minute}:{now.Second}]";
    }
  }
}