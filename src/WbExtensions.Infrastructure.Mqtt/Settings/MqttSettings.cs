namespace WbExtensions.Infrastructure.Mqtt.Settings;

internal sealed record MqttSettings(string ClientPrefix, string Host, int Port);