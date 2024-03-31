namespace dotmeer.WbExtensions.Infrastructure.Mqtt.Abstractions;

public record QueueConnection
{
    private readonly string _clientName;

    private QueueConnection(string topic, string clientName, MqttServer mqttServer)
    {
        Topic = topic;
        _clientName = clientName;
        MqttServer = mqttServer;
    }

    public string Topic { get; init; }

    public string ClientName => $"{_clientName}_{MqttServer}";

    public MqttServer MqttServer { get; init; }

    public static QueueConnection WirenBoard(string topic, string clientName)
    {
        return new QueueConnection(topic, clientName, MqttServer.WirenBoard);
    }

    public static QueueConnection Yandex(string topic, string clientName)
    {
        return new QueueConnection(topic, clientName, MqttServer.Yandex);
    }
}