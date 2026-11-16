using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using BandUp.Api.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace BandUp.IntegrationTests.Security;

/// <summary>
/// Tests the security baseline (BAND-32) on a minimal app with throwaway endpoints, so
/// each rule is checked in isolation from real features.
/// </summary>
public sealed class ApiSecurityTests : IAsyncLifetime
{
    private WebApplication _app = null!;
    private HttpClient _client = null!;

    public async Task InitializeAsync()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = "Production" });
        builder.WebHost.UseTestServer();
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["RateLimiting:PostingPerMinute"] = "2",
        });

        builder.Services.AddApiSecurity(builder.Configuration);

        // Swap the real issuer (Clerk, via JWKS) for a local test key.
        builder.Services.TrustTestTokens();

        _app = builder.Build();
        _app.UseApiErrorHandling();
        _app.UseApiAccessControl();

        _app.MapGet("/api/v1/secure", (HttpContext context) => context.User.Identity!.Name);
        _app.MapGet("/api/v1/open", () => "open").AllowAnonymous();
        _app.MapGet("/api/v1/boom", string () => throw new InvalidOperationException("db password hunter2"))
            .AllowAnonymous();
        _app.MapPost("/api/v1/posts", () => Results.Accepted()).RequireRateLimiting(RateLimitPolicies.Posting);

        await _app.StartAsync();
        _client = _app.GetTestClient();
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _app.DisposeAsync();
    }

    [Fact]
    public async Task Endpoints_require_sign_in_by_default()
    {
        var response = await _client.GetAsync("/api/v1/secure");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Anonymous_endpoints_must_opt_in()
    {
        var response = await _client.GetAsync("/api/v1/open");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task A_valid_token_is_accepted_and_identifies_the_user()
    {
        var response = await SendAsync(HttpMethod.Get, "/api/v1/secure", Token(DateTime.UtcNow.AddMinutes(5)));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("user_123", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task An_expired_token_is_rejected()
    {
        var response = await SendAsync(HttpMethod.Get, "/api/v1/secure", Token(DateTime.UtcNow.AddMinutes(-10)));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task A_token_signed_with_another_key_is_rejected()
    {
        var forged = Token(DateTime.UtcNow.AddMinutes(5), new SymmetricSecurityKey(RandomNumberGenerator.GetBytes(32)));

        var response = await SendAsync(HttpMethod.Get, "/api/v1/secure", forged);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Errors_never_leak_internal_details()
    {
        var response = await _client.GetAsync("/api/v1/boom");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        Assert.DoesNotContain("hunter2", body, StringComparison.Ordinal);
        Assert.DoesNotContain("InvalidOperationException", body, StringComparison.Ordinal);
        Assert.Contains("traceId", body, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Responses_carry_security_headers()
    {
        var response = await _client.GetAsync("/api/v1/open");

        Assert.Equal("nosniff", response.Headers.GetValues("X-Content-Type-Options").Single());
        Assert.Equal("DENY", response.Headers.GetValues("X-Frame-Options").Single());
        Assert.Equal("no-referrer", response.Headers.GetValues("Referrer-Policy").Single());
        Assert.Equal("default-src 'none'; frame-ancestors 'none'",
            response.Headers.GetValues("Content-Security-Policy").Single());
        Assert.True(response.Headers.CacheControl?.NoStore);
    }

    [Fact]
    public async Task Rate_limits_apply_per_user_with_retry_after()
    {
        var alice = Token(DateTime.UtcNow.AddMinutes(5), subject: "alice");
        var bob = Token(DateTime.UtcNow.AddMinutes(5), subject: "bob");

        Assert.Equal(HttpStatusCode.Accepted, (await SendAsync(HttpMethod.Post, "/api/v1/posts", alice)).StatusCode);
        Assert.Equal(HttpStatusCode.Accepted, (await SendAsync(HttpMethod.Post, "/api/v1/posts", alice)).StatusCode);

        var limited = await SendAsync(HttpMethod.Post, "/api/v1/posts", alice);
        Assert.Equal(HttpStatusCode.TooManyRequests, limited.StatusCode);
        Assert.NotNull(limited.Headers.RetryAfter);
        var problem = await limited.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        Assert.Equal("Too many requests", problem!["title"].ToString());

        // Another user has their own allowance.
        Assert.Equal(HttpStatusCode.Accepted, (await SendAsync(HttpMethod.Post, "/api/v1/posts", bob)).StatusCode);
    }

    private Task<HttpResponseMessage> SendAsync(HttpMethod method, string url, string token)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return _client.SendAsync(request);
    }

    private static string Token(DateTime expires, SecurityKey? key = null, string subject = "user_123") =>
        TestTokens.Create(subject, expires, key, authorizedParty: null);
}
