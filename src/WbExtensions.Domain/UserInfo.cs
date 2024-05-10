using System;

namespace WbExtensions.Domain;

public sealed record UserInfo(string Id, string Email, DateTime Updated);