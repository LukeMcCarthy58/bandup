using BandUp.Application.Reference;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BandUp.Api.Endpoints;

/// <summary>Public lookups for pickers and autocomplete. No sign-in needed.</summary>
internal static class ReferenceDataEndpoints
{
    public static RouteGroupBuilder MapReferenceDataEndpoints(this RouteGroupBuilder api)
    {
        api.MapGet("/instruments", async (IReferenceDataQueries queries, HttpContext context, CancellationToken ct) =>
            {
                CacheForAnHour(context);
                return TypedResults.Ok(await queries.GetInstrumentsAsync(ct));
            })
            .AllowAnonymous()
            .WithTags("Reference data")
            .WithSummary("All instruments, in picker order");

        api.MapGet("/genres", async (IReferenceDataQueries queries, HttpContext context, CancellationToken ct) =>
            {
                CacheForAnHour(context);
                return TypedResults.Ok(await queries.GetGenresAsync(ct));
            })
            .AllowAnonymous()
            .WithTags("Reference data")
            .WithSummary("All genres, alphabetical");

        api.MapGet("/towns", SearchTownsAsync)
            .AllowAnonymous()
            .WithTags("Reference data")
            .WithSummary("Towns starting with the query, for autocomplete. Never returns coordinates.");

        return api;
    }

    private static async Task<Results<Ok<IReadOnlyList<TownSummary>>, ValidationProblem>> SearchTownsAsync(
        string? query,
        int? limit,
        IReferenceDataQueries queries,
        CancellationToken ct)
    {
        var term = query?.Trim() ?? string.Empty;
        if (term.Length is < TownSearch.MinQueryLength or > TownSearch.MaxQueryLength)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["query"] =
                [
                    $"Enter between {TownSearch.MinQueryLength} and {TownSearch.MaxQueryLength} characters.",
                ],
            });
        }

        var towns = await queries.SearchTownsAsync(term, TownSearch.ClampLimit(limit), ct);
        return TypedResults.Ok(towns);
    }

    /// <summary>Reference data rarely changes and isn't personal, so browsers may cache it.</summary>
    private static void CacheForAnHour(HttpContext context) =>
        context.Response.Headers.CacheControl = "public, max-age=3600";
}
