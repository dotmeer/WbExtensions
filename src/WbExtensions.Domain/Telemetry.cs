using System;

namespace WbExtensions.Domain;

public sealed record Telemetry(string Device, string Control, string Value, DateTime Updated);