namespace WbExtensions.Domain.Alice.Responses;

public sealed class AliceResponseWithPayload : AliceResponse
{
    public Payload Payload { get; init; } = default!;
}