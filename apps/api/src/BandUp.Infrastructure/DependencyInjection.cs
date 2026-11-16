using BandUp.Application.Reference;
using BandUp.Application.Users;
using BandUp.Infrastructure.Persistence;
using BandUp.Infrastructure.Reference;
using BandUp.Infrastructure.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BandUp.Infrastructure;

public static class DependencyInjection
{
    public const string ConnectionStringName = "BandUp";

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConnectionStringName);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Connection string '{ConnectionStringName}' is not configured. " +
                "Set it with 'dotnet user-secrets' (see apps/api/README.md).");
        }

        services.AddDbContext<BandUpDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddHealthChecks()
            .AddDbContextCheck<BandUpDbContext>("database", tags: ["ready"]);

        services.AddSingleton(TimeProvider.System);
        services.AddScoped<IReferenceDataQueries, ReferenceDataQueries>();
        services.AddScoped<IUserAccounts, UserAccounts>();

        return services;
    }
}
