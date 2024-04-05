namespace WbExtensions.Infrastructure.Mqtt.Settings;

internal sealed record MqttSettings(
    string ClientPrefix,
    WbMqttConnectionSettings Wb,
    YandexMqttConnectionSettings Yandex);