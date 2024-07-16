namespace WbExtensions.Domain.Mqtt;

public record struct QueueMessage(string Topic, string? Payload);