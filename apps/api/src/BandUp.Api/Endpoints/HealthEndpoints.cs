using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace BandUp.Api.Endpoints;

internal static class HealthEndpoints
{
    public static IEndpointRouteBuilder MapHealthEndpoints(this IEndpointRouteBuilder app)
    {
        // Liveness: the process is up. No dependencies checked.
        app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false })
            .AllowAnonymous()
            .DisableRateLimiting();

        // Readiness: the API can serve requests, including reaching the database.
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready"),
        })
            .AllowAnonymous()
            .DisableRateLimiting();

        return app;
    }
}
