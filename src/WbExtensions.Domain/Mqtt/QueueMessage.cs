namespace WbExtensions.Domain.Mqtt;

public record QueueMessage(string Topic, string? Payload);