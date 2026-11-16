using System.Security.Claims;

namespace BandUp.Api.Security;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// The auth provider's user id ("sub"). Only call on authenticated endpoints; the
    /// fallback policy guarantees a validated token, which always has a subject.
    /// </summary>
    public static string GetAuthSubject(this ClaimsPrincipal user) =>
        user.FindFirstValue("sub")
        ?? throw new InvalidOperationException("Authenticated user has no 'sub' claim.");
}
