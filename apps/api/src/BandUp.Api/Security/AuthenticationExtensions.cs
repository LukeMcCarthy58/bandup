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
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        if (!IsAuthorizedParty(context.Principal?.FindFirst("azp")?.Value, settings.AuthorizedParties))
                        {
                            context.Fail("Token was issued to an origin that isn't allowed.");
                        }

                        return Task.CompletedTask;
                    },
                };
            });

        return services;
    }

    /// <summary>
    /// If allowed parties are configured, a token that names its requesting origin ("azp")
    /// must name one of them. Tokens without "azp" (e.g. machine tokens) aren't affected.
    /// </summary>
    internal static bool IsAuthorizedParty(string? azp, IReadOnlyCollection<string> allowed) =>
        allowed.Count == 0
        || azp is null
        || allowed.Contains(azp.TrimEnd('/'), StringComparer.OrdinalIgnoreCase);
}
