using System.Security.Claims;
using BandUp.Api.Security;
using BandUp.Application.Users;

namespace BandUp.Api.Endpoints;

internal static class MeEndpoints
{
    public static RouteGroupBuilder MapMeEndpoints(this RouteGroupBuilder api)
    {
        // Signed-in only (fallback policy). Creates the BandUp account on the first call.
        api.MapGet("/me", async (ClaimsPrincipal user, IUserAccounts accounts, CancellationToken ct) =>
                TypedResults.Ok(await accounts.GetOrCreateAsync(user.GetAuthSubject(), ct)))
            .WithTags("Me")
            .WithSummary("The signed-in user's BandUp account, created on first call");

        // Deletes only the caller's own account: the subject comes from their validated token,
        // never from the request. 204 even if there was nothing to delete, so retries are safe.
        api.MapDelete("/me", async (ClaimsPrincipal user, IUserAccounts accounts, CancellationToken ct) =>
            {
                await accounts.DeleteAsync(user.GetAuthSubject(), ct);
                return TypedResults.NoContent();
            })
            .RequireRateLimiting(RateLimitPolicies.Sensitive)
            .WithTags("Me")
            .WithSummary("Permanently delete your BandUp account and profile");

        return api;
    }
}
