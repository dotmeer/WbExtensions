namespace WbExtensions.Infrastructure.Mqtt.Settings;

internal abstract record MqttConnectionSettings(string Host, int Port);