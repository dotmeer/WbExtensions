using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WbExtensions.Application.Interfaces.Database;
using WbExtensions.Application.Interfaces.Yandex;
using WbExtensions.Domain;
using WbExtensions.Infrastructure.Yandex.Models;
using WbExtensions.Infrastructure.Yandex.Settings;

namespace WbExtensions.Infrastructure.Yandex.Implementations;

internal sealed class UserService : IUserService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly UserServiceSettings _settings;
    private readonly IUserInfoRepository _userInfoRepository;
    private readonly ConcurrentDictionary<string, YandexUserInfo> _users;
    
    public UserService(
        IHttpClientFactory httpClientFactory, 
        UserServiceSettings settings,
        IUserInfoRepository userInfoRepository)
    {
        _httpClientFactory = httpClientFactory;
        _settings = settings;
        _userInfoRepository = userInfoRepository;
        _users = new ConcurrentDictionary<string, YandexUserInfo>();
    }

    public async Task<string?> GetUserIdAsync(string? token, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(token))
        {
            return null;
        }
        
        YandexUserInfo? userInfo;
        if (_users.TryGetValue(token, out userInfo))
        {
            return userInfo.Id;
        }

        userInfo = await GetInfoAsync(token, cancellationToken);

        if (userInfo is null)
        {
            return null;
        }

        if (!_settings.AllowedUsers.Contains(userInfo.DefaultEmail, StringComparer.OrdinalIgnoreCase))
        {
            return null;
        }

        _users.AddOrUpdate(
            token,
            _ => userInfo,
            (t, v) => userInfo);

        await _userInfoRepository.UpsertAsync(
            new UserInfo(userInfo.Id, userInfo.DefaultEmail, DateTime.UtcNow),
            cancellationToken);

        return userInfo.Id;
    }

    public async Task RemoveAsync(string? token, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(token)
            && _users.TryRemove(token.Replace("Bearer ", ""), out var userInfo))
        {
            await _userInfoRepository.RemoveAsync(userInfo.Id, cancellationToken);
        }
    }

    private async Task<YandexUserInfo?> GetInfoAsync(string token, CancellationToken cancellationToken)
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