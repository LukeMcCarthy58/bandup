using BandUp.Domain.Common;

namespace BandUp.Domain.Users;

/// <summary>
/// A BandUp account. Identity (passwords, Google sign-in, sessions) lives in the auth
/// provider; this links its user to BandUp's own data. Profile details arrive in BAND-37.
/// </summary>
public sealed class User : Entity
{
    public const int AuthSubjectMaxLength = 64;

    // For EF Core.
    private User()
    {
    }

    private User(string authSubject)
    {
        AuthSubject = authSubject;
        LastSeenAt = CreatedAt;
    }

    /// <summary>The auth provider's stable user id (the token's "sub" claim), e.g. Clerk's "user_…".</summary>
    public string AuthSubject { get; private set; } = string.Empty;

    public DateTimeOffset LastSeenAt { get; private set; }

    public static User Create(string authSubject)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(authSubject);
        if (authSubject.Length > AuthSubjectMaxLength)
        {
            throw new ArgumentException($"Auth subject is longer than {AuthSubjectMaxLength} characters.", nameof(authSubject));
        }

        return new User(authSubject);
    }

    /// <summary>Records activity, at most once per <paramref name="granularity"/> to avoid a write per request.</summary>
    public bool MarkSeen(DateTimeOffset now, TimeSpan granularity)
    {
        if (now - LastSeenAt < granularity)
        {
            return false;
        }

        LastSeenAt = Timestamps.ToStoragePrecision(now);
        return true;
    }
}
