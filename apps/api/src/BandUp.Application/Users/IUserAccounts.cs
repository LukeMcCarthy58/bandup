namespace BandUp.Application.Users;

/// <summary>The signed-in user's BandUp account, as returned by GET /me.</summary>
public sealed record CurrentUser(Guid Id, DateTimeOffset CreatedAt);

public interface IUserAccounts
{
    /// <summary>
    /// Finds the BandUp account for a signed-in user, creating it on their first request.
    /// Safe to call concurrently for the same user.
    /// </summary>
    Task<CurrentUser> GetOrCreateAsync(string authSubject, CancellationToken cancellationToken);

    /// <summary>
    /// Permanently deletes the user's BandUp account and everything that belongs only to them
    /// (profile, instruments, genres). Returns false if they had no account. See ADR 0014.
    /// </summary>
    Task<bool> DeleteAsync(string authSubject, CancellationToken cancellationToken);
}
