using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WhatsAppMessagesApiNet.Domain.Interfaces;

namespace WhatsAppMessagesApiNet.Infrastructure.Providers;

public class TwilioWhatsAppProvider : IWhatsAppProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TwilioWhatsAppProvider> _logger;
    private readonly string _accountSid;
    private readonly string _authToken;
    private readonly string _fromNumber;

    public string Name => "Twilio";

    public TwilioWhatsAppProvider(HttpClient httpClient, IConfiguration configuration, ILogger<TwilioWhatsAppProvider> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        var twilioSection = configuration.GetSection("WhatsAppProviders:Twilio");
        _accountSid = twilioSection["AccountSid"] ?? "";
        _authToken = twilioSection["AuthToken"] ?? "";
        _fromNumber = twilioSection["FromNumber"] ?? "";
    }

    public async Task<WhatsAppMessageResult> SendTextAsync(string to, string text)
    {
        try
        {
            var from = $"whatsapp:{_fromNumber}";
            var toNumber = to.Contains("whatsapp:") ? to : $"whatsapp:{to}";

            var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_accountSid}:{_authToken}"));
            var request = new HttpRequestMessage(HttpMethod.Post,
                $"https://api.twilio.com/2010-04-01/Accounts/{_accountSid}/Messages.json")
            {
                Headers = { { "Authorization", $"Basic {auth}" } },
                Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("To", toNumber),
                    new KeyValuePair<string, string>("From", from),
                    new KeyValuePair<string, string>("Body", text)
                })
            };

            var response = await _httpClient.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var json = JsonDocument.Parse(body);
                var messageId = json.RootElement.GetProperty("sid").GetString();
                _logger.LogInformation("[Twilio] Message sent: {MessageId}", messageId);
                return new WhatsAppMessageResult { Success = true, MessageId = messageId };
            }

            _logger.LogError("[Twilio] Error {Status}: {Body}", response.StatusCode, body);
            return new WhatsAppMessageResult { Success = false, ErrorMessage = body };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Twilio] Exception");
            return new WhatsAppMessageResult { Success = false, ErrorMessage = ex.Message };
        }
    }
}
