using BandUp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace BandUp.IntegrationTests;

/// <summary>
/// Boots the real API in memory against a dedicated test database, so tests never
/// touch your dev data. The server, user and password come from the
/// ConnectionStrings__BandUp environment variable (CI) or the API's user-secrets
/// (your machine); the database name is always swapped to bandup_test.
/// Migrations are applied automatically.
/// </summary>
public sealed class BandUpApiFactory : WebApplicationFactory<Program>
{
    public const string TestDatabaseName = "bandup_test";

    // Test classes run in parallel; migrate once per test run, not once per class.
    private static readonly Lock MigrationLock = new();
    private static bool _migrated;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.UseSetting("ConnectionStrings:BandUp", TestConnectionString());
        builder.ConfigureServices(services => services.TrustTestTokens());
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        lock (MigrationLock)
        {
            if (!_migrated)
            {
                using var scope = host.Services.CreateScope();
                scope.ServiceProvider.GetRequiredService<BandUpDbContext>().Database.Migrate();
                _migrated = true;
            }
        }

        return host;
    }

    private static string TestConnectionString()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<Program>(optional: true)
            .AddEnvironmentVariables()
            .Build();

        var source = configuration.GetConnectionString("BandUp");
        if (string.IsNullOrWhiteSpace(source))
        {
            throw new InvalidOperationException(
                "No database for integration tests. Set the API's user-secret " +
                "'ConnectionStrings:BandUp' (see apps/api/README.md) or the " +
                "ConnectionStrings__BandUp environment variable.");
        }

        return new NpgsqlConnectionStringBuilder(source) { Database = TestDatabaseName }.ConnectionString;
    }
}
