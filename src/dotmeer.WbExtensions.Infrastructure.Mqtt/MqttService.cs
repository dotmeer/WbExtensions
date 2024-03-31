using dotmeer.WbExtensions.Infrastructure.Mqtt.Abstractions;
using dotmeer.WbExtensions.Infrastructure.Mqtt.Settings;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;

namespace dotmeer.WbExtensions.Infrastructure.Mqtt;

internal sealed class MqttService : IMqttService, IDisposable
{
    private readonly ILogger<MqttService> _logger;

    private readonly MqttSettings _mqttSettings;

    private readonly MqttFactory _mqttFactory;

    private readonly IDictionary<string, IMqttClient> _mqttClients;

    public MqttService(
        ILogger<MqttService> logger,
        MqttSettings mqttSettings)
    {
        _logger = logger;
        _mqttSettings = mqttSettings;
        _mqttFactory = new MqttFactory();
        _mqttClients = new ConcurrentDictionary<string, IMqttClient>();
    }

    public async Task PublishAsync(
        QueueConnection connection,
        string payload,
        CancellationToken cancellationToken)
    {
        if (!_mqttClients.TryGetValue(connection.ClientName, out var mqttClient))
        {
            mqttClient = _mqttFactory.CreateMqttClient();

            var options = CreateOptions(connection);

            mqttClient.ConnectedAsync += _ =>
            {
                _logger.LogInformation("Connected to topic {Topic}", connection.Topic);
                return Task.CompletedTask;
            };

            mqttClient.DisconnectedAsync += _ =>
                ReconnectAsync(connection.ClientName, mqttClient, options, cancellationToken);

            await mqttClient.ConnectAsync(options, cancellationToken);

            _mqttClients[connection.ClientName] = mqttClient;
        }

        var message = new MqttApplicationMessageBuilder()
            .WithTopic(connection.Topic)
            .WithPayload(payload)
            .WithRetainFlag()
            .Build();

        await mqttClient.PublishAsync(message, cancellationToken);
    }

    public async Task SubscribeAsync(
        QueueConnection connection,
        Func<QueueMessage, CancellationToken, Task> receiveHandler,
        CancellationToken cancellationToken)
    {
        if (!_mqttClients.TryGetValue(connection.ClientName, out var mqttClient))
        {
            mqttClient = _mqttFactory.CreateMqttClient();

            var options = CreateOptions(connection);

            mqttClient.ConnectedAsync += _ =>
            {
                var topicFilter = new MqttTopicFilterBuilder()
                    .WithTopic(connection.Topic)
                    .Build();
                _logger.LogInformation("Connected to topic {Topic}", connection.Topic);
                return mqttClient.SubscribeAsync(topicFilter, cancellationToken);
            };

            mqttClient.DisconnectedAsync += _ =>
                ReconnectAsync(connection.ClientName, mqttClient, options, cancellationToken);

            mqttClient.ApplicationMessageReceivedAsync += _ =>
                receiveHandler(
                    new QueueMessage(_.ApplicationMessage.Topic, _.ApplicationMessage.ConvertPayloadToString()),
                    cancellationToken);

            await mqttClient.ConnectAsync(options, cancellationToken);

            _mqttClients[connection.ClientName] = mqttClient;
        }
    }

    public void Dispose()
    {
        foreach (var mqttClient in _mqttClients.Values)
        {
            mqttClient.Dispose();
        }
    }

    private MqttClientOptions CreateOptions(QueueConnection connection)
    {
        var options = new MqttClientOptionsBuilder()
            .WithClientId($"{_mqttSettings.ClientPrefix}_{connection.ClientName}");

        switch (connection.MqttServer)
        {
            case MqttServer.WirenBoard:
                options = options
                    .WithTcpServer(_mqttSettings.Wb.Host, _mqttSettings.Wb.Port);
                break;

            case MqttServer.Yandex:
                options = options
                    .WithTcpServer(_mqttSettings.Yandex.Host, _mqttSettings.Yandex.Port)
                    .WithTlsOptions(new MqttClientTlsOptions
                    {
                        UseTls = true,
                        SslProtocol = SslProtocols.Tls12,
                        CertificateValidationHandler = args => true
                    })
                    .WithCredentials(_mqttSettings.Yandex.Login, _mqttSettings.Yandex.Password);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(connection.MqttServer), connection.MqttServer, null);
        }

        return options
            .WithKeepAlivePeriod(TimeSpan.FromSeconds(90))
            .WithCleanSession()
            .Build();
    }

    private async Task ReconnectAsync(
        string topic,
        IMqttClient mqttClient,
        MqttClientOptions options,
        CancellationToken cancellationToken)
    {
        _logger.LogWarning("Reconnecting to topic {Topic}.", topic);

        await Task.Delay(TimeSpan.FromMinutes(5), cancellationToken);

        try
        {
            await mqttClient.ConnectAsync(options, cancellationToken);
        }
        catch
        {
            _logger.LogError("Reconnect failed to topic {Topic}.", topic);
        }
    }
}