using System;

namespace WbExtensions.Application.Helpers;

public static class TopicNameHelper
{
    public static (string deviceName, string topicName) ParseDeviceControlTopic(string topic)
    {
        var topicParts = topic.Split("/", StringSplitOptions.RemoveEmptyEntries);

        return (topicParts[1], topicParts[3]);
    }

    public static string GetZigbee2MqttDevice(string topic)
    {
        var topicParts = topic.Split("/");
        return topicParts[1];
    }
}