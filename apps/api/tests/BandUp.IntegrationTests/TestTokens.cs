using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace BandUp.IntegrationTests;

/// <summary>
/// Signs tokens the way Clerk would, with a local key, so tests can act as signed-in users
/// without calling Clerk.
/// </summary>
public static class TestTokens
{
    public const string Issuer = "https://issuer.test";
    public const string WebOrigin = "http://localhost:3000";

    public static readonly SymmetricSecurityKey SigningKey = new(RandomNumberGenerator.GetBytes(32));

    /// <summary>Makes the API trust tokens from <see cref="Create"/> instead of the real issuer.</summary>
    public static IServiceCollection TrustTestTokens(this IServiceCollection services) =>
        services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.Authority = null;
            options.TokenValidationParameters.ValidIssuer = Issuer;
            options.TokenValidationParameters.IssuerSigningKey = SigningKey;
        });

    public static string Create(
        string subject = "user_123",
        DateTime? expires = null,
        SecurityKey? key = null,
        string? authorizedParty = WebOrigin)
    {
        var expiry = expires ?? DateTime.UtcNow.AddMinutes(5);
        var claims = new List<Claim> { new("sub", subject) };
        if (authorizedParty is not null)
        {
            claims.Add(new Claim("azp", authorizedParty));
        }

        return new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor
        {
            Issuer = Issuer,
            Subject = new ClaimsIdentity(claims),
            IssuedAt = expiry.AddHours(-1),
            NotBefore = expiry.AddHours(-1),
            Expires = expiry,
            SigningCredentials = new SigningCredentials(key ?? SigningKey, SecurityAlgorithms.HmacSha256),
        });
    }

    public static HttpRequestMessage Authorized(this HttpRequestMessage request, string token)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return request;
    }

    /// <summary>A unique subject per test, because the test database persists between runs.</summary>
    public static string NewSubject() => $"user_test_{Guid.NewGuid():N}";
}
