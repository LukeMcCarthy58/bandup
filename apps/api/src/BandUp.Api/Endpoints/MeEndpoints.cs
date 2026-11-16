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

        return api;
    }
}
