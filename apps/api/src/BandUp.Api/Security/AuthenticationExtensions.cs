using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace BandUp.Api.Security;

internal static class AuthenticationExtensions
{
    /// <summary>
    /// Bearer-token authentication. Tokens are validated against the issuer's published
    /// signing keys (JWKS): signature, issuer, lifetime and, if configured, audience.
    /// </summary>
    public static IServiceCollection AddBearerAuthentication(
        this IServiceCollection services,
        AuthSettings settings)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = string.IsNullOrWhiteSpace(settings.Authority) ? null : settings.Authority;
                options.MapInboundClaims = false;
                options.TokenValidationParameters.NameClaimType = "sub";
                options.TokenValidationParameters.ValidateAudience =
                    !string.IsNullOrWhiteSpace(settings.Audience);
                options.TokenValidationParameters.ValidAudience = settings.Audience;
            });

        return services;
    }
}
