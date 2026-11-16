namespace BandUp.Api.Security;

/// <summary>Who issues user tokens. Filled in with Clerk's values in M2 (BAND-35).</summary>
public sealed class AuthSettings
{
    public const string SectionName = "Auth";

    /// <summary>Token issuer, e.g. the Clerk Frontend API URL. Its JWKS signs the tokens.</summary>
    public string? Authority { get; init; }

    /// <summary>Expected audience. Leave empty if the issuer doesn't set one.</summary>
    public string? Audience { get; init; }
}
