namespace WbExtensions.Domain.Home;

public sealed record Command(
    string Device,
    string Control,
    string Value);