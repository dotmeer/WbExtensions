namespace WbExtensions.Application.UseCases.UpdateVirtualDeviceState;

public sealed record UpdateVirtualDeviceStateRequest(
    string Topic,
    string? Value);