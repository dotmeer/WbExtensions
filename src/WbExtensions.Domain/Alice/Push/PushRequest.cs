using System;

namespace WbExtensions.Domain.Alice.Push;

public sealed class PushRequest
{
    public PushRequest(PushRequestPayload payload)
    {
        Ts = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        Payload = payload;
    }

    public double Ts { get; }

    public PushRequestPayload Payload { get; }
}