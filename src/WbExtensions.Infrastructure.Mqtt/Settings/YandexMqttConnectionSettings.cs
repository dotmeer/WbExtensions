namespace WbExtensions.Infrastructure.Mqtt.Settings;

internal sealed record YandexMqttConnectionSettings(string Host, int Port, string Login, string Password)
    : MqttConnectionSettings(Host, Port);