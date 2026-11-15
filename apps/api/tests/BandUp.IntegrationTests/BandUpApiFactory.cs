using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BandUp.IntegrationTests;

/// <summary>
/// Boots the real API in memory. Uses the ConnectionStrings__BandUp environment
/// variable when set (CI Postgres service, or your local database); otherwise a
/// placeholder that is never connected to, so tests that don't touch the
/// database still run anywhere.
/// </summary>
public sealed class BandUpApiFactory : WebApplicationFactory<Program>
{
    public static string? DatabaseConnectionString =>
        Environment.GetEnvironmentVariable("ConnectionStrings__BandUp");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.UseSetting(
            "ConnectionStrings:BandUp",
            DatabaseConnectionString ?? "Host=localhost;Database=bandup_unused;Username=none");
    }
}
