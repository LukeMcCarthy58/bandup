using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using BandUp.Api.Security;
using BandUp.Application.Profiles;
using BandUp.Domain.Profiles;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BandUp.Api.Endpoints;

/// <summary>
/// The request body for saving your profile. Shape and length rules are checked here
/// (400 before any work); rules that need the database are checked by IProfiles.
/// </summary>
public sealed record UpdateProfileRequest(
    [property: Required(ErrorMessage = "Enter your name.")]
    [property: StringLength(
        ProfileRules.DisplayNameMaxLength,
        MinimumLength = ProfileRules.DisplayNameMinLength,
        ErrorMessage = "Enter a name between 2 and 60 characters.")]
    string DisplayName,
    [property: StringLength(ProfileRules.BioMaxLength, ErrorMessage = "Keep your bio under 500 characters.")]
    string? Bio,
    [property: Required(ErrorMessage = "Choose your town from the list.")]
    [property: Range(1, int.MaxValue, ErrorMessage = "Choose your town from the list.")]
    int? TownId,
    [property: Required(ErrorMessage = "Choose your level.")]
    ExperienceLevel? Level,
    [property: Required(ErrorMessage = "Choose how much time you can give a band.")]
    Commitment? Commitment,
    [property: Required(ErrorMessage = "Choose at least one instrument.")]
    [property: MinLength(1, ErrorMessage = "Choose at least one instrument.")]
    [property: MaxLength(ProfileRules.MaxInstruments, ErrorMessage = "Choose up to 10 instruments.")]
    int[] InstrumentIds,
    [property: MaxLength(ProfileRules.MaxGenres, ErrorMessage = "Choose up to 10 genres.")]
    int[]? GenreIds);

internal static class ProfileEndpoints
{
    public static RouteGroupBuilder MapProfileEndpoints(this RouteGroupBuilder api)
    {
        api.MapGet("/me/profile", async (ClaimsPrincipal user, IProfiles profiles, CancellationToken ct) =>
                TypedResults.Ok(await profiles.GetMineAsync(user.GetAuthSubject(), ct)))
            .WithTags("Profiles")
            .WithSummary("Your own profile (empty until you set it up)");

        api.MapPut("/me/profile", UpdateMyProfileAsync)
            .RequireRateLimiting(RateLimitPolicies.Posting)
            .WithTags("Profiles")
            .WithSummary("Create or update your profile");

        api.MapGet("/users/{id:guid}", GetProfileAsync)
            .WithTags("Profiles")
            .WithSummary("Another musician's profile. Never includes email or exact location.");

        return api;
    }

    private static async Task<Results<Ok<ProfileView>, ValidationProblem>> UpdateMyProfileAsync(
        UpdateProfileRequest request,
        ClaimsPrincipal user,
        IProfiles profiles,
        CancellationToken ct)
    {
        var input = new ProfileInput(
            request.DisplayName,
            request.Bio,
            request.TownId!.Value,
            request.Level!.Value,
            request.Commitment!.Value,
            request.InstrumentIds,
            request.GenreIds ?? []);

        var result = await profiles.UpdateMineAsync(user.GetAuthSubject(), input, ct);
        return result.Succeeded
            ? TypedResults.Ok(result.Profile!)
            : TypedResults.ValidationProblem(result.Errors);
    }

    private static async Task<Results<Ok<ProfileView>, NotFound>> GetProfileAsync(
        Guid id,
        IProfiles profiles,
        CancellationToken ct)
    {
        var profile = await profiles.GetPublicAsync(id, ct);
        return profile is null ? TypedResults.NotFound() : TypedResults.Ok(profile);
    }
}
