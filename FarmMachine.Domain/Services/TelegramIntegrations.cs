using System;
using System.Threading.Tasks;
using Serilog;
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
    }

    public async Task Start(Func<string> callbackCodeAuth)
    {
      if (!_settings.Enabled)
      {
        return;
      }

      _client = new TelegramClient(_settings.ApiKey, _settings.ApiHash, _store, "session");
      var isUserAuthorized = _client.IsUserAuthorized();
      
      Log.Information("Begin connect to telegram");

      try
      {
        await _client.ConnectAsync();

        var hash = await _client.SendCodeRequestAsync(_settings.PhoneNumber);
        var code = callbackCodeAuth();

        _user = await _client.MakeAuthAsync(_settings.PhoneNumber, hash, code);
        
        Log.Information("Telegram api connected");
      }
      catch (Exception ex)
      {
        Log.Error(ex.ToString());
      }
    }

    public async Task SendInfo(string text)
    {
      if (!_settings.Enabled)
      {
        return;
      }

      try
      {
        await _client.SendMessageAsync(new TLInputPeerSelf(), $"{GetFormatDateTime()} [INFO] {text}");
      }
      catch (Exception ex)
      {
        Log.Error(ex.ToString());
      }
    }

    public async Task SendWarn(string text)
    {
      if (!_settings.Enabled)
      {
        return;
      }

      try
      {
        await _client.SendMessageAsync(new TLInputPeerSelf(), $"{GetFormatDateTime()} [WARN] {text}");
      }
      catch (Exception ex)
      {
        Log.Error(ex.ToString());
      }
    }
    
    public async Task SendError(Exception ex)
    {
      if (!_settings.Enabled)
      {
        return;
      }

      try
      {
        await _client.SendMessageAsync(new TLInputPeerSelf(), $"{GetFormatDateTime()} [ERROR] {ex.ToString()}");
      }
      catch (Exception e)
      {
        Log.Error(e.ToString());
      }
    }

    private string GetFormatDateTime()
    {
      var now = DateTime.Now;
      
      return $"[{now.Day}.{now.Month}.{now.Year} {now.Hour}:{now.Minute}:{now.Second}]";
    }
  }
}