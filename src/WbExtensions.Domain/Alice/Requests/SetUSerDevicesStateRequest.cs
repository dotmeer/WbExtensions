namespace WbExtensions.Domain.Alice.Requests;

public sealed class SetUSerDevicesStateRequest
{
    public SetUSerDevicesStateRequestPayload Payload { get; init; } = new();
}