using Microsoft.Extensions.DependencyInjection;

namespace BandUp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Application services are registered here as features are added.
        return services;
    }
}
