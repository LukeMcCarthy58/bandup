namespace BandUp.Api.Security;

/// <summary>Names of the rate-limit policies endpoints opt into with RequireRateLimiting.</summary>
public static class RateLimitPolicies
{
    /// <summary>Creating posts, comments, adverts and bands.</summary>
    public const string Posting = "posting";

    /// <summary>Sending messages and join requests.</summary>
    public const string Messaging = "messaging";

    /// <summary>Abuse-prone actions such as reports and account deletion.</summary>
    public const string Sensitive = "sensitive";
}

/// <summary>Requests allowed per minute, per signed-in user (or per IP when anonymous).</summary>
public sealed class RateLimitSettings
{
    public const string SectionName = "RateLimiting";

    public int GlobalPerMinute { get; init; } = 300;

    public int PostingPerMinute { get; init; } = 10;

    public int MessagingPerMinute { get; init; } = 30;

    public int SensitivePerMinute { get; init; } = 5;
}
