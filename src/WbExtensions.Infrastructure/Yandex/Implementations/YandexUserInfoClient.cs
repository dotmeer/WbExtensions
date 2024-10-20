using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WbExtensions.Application.Interfaces.Yandex;
using WbExtensions.Application.Models.Yandex;

namespace WbExtensions.Infrastructure.Yandex.Implementations;

internal sealed class YandexUserInfoClient : IYandexUserInfoClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public YandexUserInfoClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<YandexUserInfo?> GetInfoAsync(string token, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            "https://login.yandex.ru/info?fotmat=json");
        request.Headers.Add("Authorization", $"OAuth {token}");

        using var httpClient = _httpClientFactory.CreateClient();

        var response = await httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await JsonSerializer.DeserializeAsync<YandexUserInfo>(
            await response.Content.ReadAsStreamAsync(cancellationToken),
            cancellationToken: cancellationToken);
    }
}