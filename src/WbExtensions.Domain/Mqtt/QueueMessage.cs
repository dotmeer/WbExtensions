namespace WbExtensions.Domain.Mqtt;

public sealed record QueueMessage(string Topic, string? Payload);