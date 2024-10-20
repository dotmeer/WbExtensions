using System.Text.Json.Serialization;

namespace WbExtensions.Application.Models.Yandex;

public sealed class YandexUserInfo
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = default!;

    [JsonPropertyName("login")]
    public string Login { get; init; } = default!;

    [JsonPropertyName("default_email")]
    public string DefaultEmail { get; init; } = default!;
}