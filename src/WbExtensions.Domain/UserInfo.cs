using System;

namespace WbExtensions.Domain;

public record struct UserInfo(string Id, string Email, DateTime Updated);