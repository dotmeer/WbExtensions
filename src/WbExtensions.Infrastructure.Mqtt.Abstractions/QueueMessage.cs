﻿namespace WbExtensions.Infrastructure.Mqtt.Abstractions;

public record QueueMessage(string Topic, string Payload);