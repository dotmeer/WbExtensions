namespace WbExtensions.Infrastructure.Yandex.Settings;

public sealed record PushServiceSettings(
    string Token,
    string SkillId);