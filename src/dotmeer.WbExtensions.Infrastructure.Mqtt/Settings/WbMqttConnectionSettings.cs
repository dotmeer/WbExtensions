namespace dotmeer.WbExtensions.Infrastructure.Mqtt.Settings;

internal sealed record WbMqttConnectionSettings(string Host, int Port)
    : MqttConnectionSettings(Host, Port);