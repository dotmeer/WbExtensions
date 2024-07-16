using System;

namespace WbExtensions.Domain;

public record struct Telemetry(string Device, string Control, string Value, DateTime Updated);