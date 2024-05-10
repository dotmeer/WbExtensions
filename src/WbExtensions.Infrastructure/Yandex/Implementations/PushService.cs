using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WbExtensions.Application.Interfaces.Yandex;
using WbExtensions.Domain.Alice.Push;
using WbExtensions.Infrastructure.Json;
using WbExtensions.Infrastructure.Yandex.Settings;

namespace WbExtensions.Infrastructure.Yandex.Implementations;

internal sealed class PushService : IPushService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly PushServiceSettings _settings;
    private readonly ILogger<PushService> _logger;

    public PushService(
        IHttpClientFactory httpClientFactory, 
        PushServiceSettings settings, 
        ILogger<PushService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _settings = settings;
        _logger = logger;
    }

    public async Task PushAsync(PushRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var httpRequest = new HttpRequestMessage(
                HttpMethod.Post,
                $"https://dialogs.yandex.net/api/v1/skills/{_settings.SkillId}/callback/state");
            httpRequest.Content = JsonContent.Create(request, options: new JsonSerializerOptions().Configure());
            httpRequest.Headers.Add("Authorization", $"OAuth {_settings.Token}");

            using var httpClient = _httpClientFactory.CreateClient();
            var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken);

            if (!httpResponse.IsSuccessStatusCode)
            {
                var response = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("Error from yandex push API: {Response}", response);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while sending push request to yandex");
        }
    }
}