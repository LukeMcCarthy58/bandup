using System.Diagnostics;
using System.Globalization;
using System.Security.Claims;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;

namespace BandUp.Api.Security;

/// <summary>
/// The security baseline every endpoint inherits (ADR 0010): problem-details errors,
/// security headers, CORS, authenticated-by-default authorisation and rate limiting.
/// </summary>
public static class ApiSecurityExtensions
{
    public const string WebCorsPolicy = "web";

    public static IServiceCollection AddApiSecurity(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Errors are RFC 7807 problem details with a trace id for support; never stack traces.
        services.AddProblemDetails(options => options.CustomizeProblemDetails = context =>
            context.ProblemDetails.Extensions["traceId"] =
                Activity.Current?.Id ?? context.HttpContext.TraceIdentifier);

        var auth = configuration.GetSection(AuthSettings.SectionName).Get<AuthSettings>() ?? new();
        services.AddBearerAuthentication(auth);

        // Every endpoint requires a signed-in user unless it opts out with AllowAnonymous().
        // Resource rules (e.g. "only this band's admins") are added as policies per feature.
        services.AddAuthorizationBuilder()
            .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        services.AddCors(options => options.AddPolicy(WebCorsPolicy, policy => policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()));

        var limits = configuration.GetSection(RateLimitSettings.SectionName).Get<RateLimitSettings>()
            ?? new();
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = WriteRateLimitedAsync;
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                PerMinute(context, limits.GlobalPerMinute));
            options.AddPolicy(RateLimitPolicies.Posting, context => PerMinute(context, limits.PostingPerMinute));
            options.AddPolicy(RateLimitPolicies.Messaging, context => PerMinute(context, limits.MessagingPerMinute));
            options.AddPolicy(RateLimitPolicies.Sensitive, context => PerMinute(context, limits.SensitivePerMinute));
        });

        return services;
    }

    /// <summary>First in the pipeline: turns exceptions and empty error responses into problem details.</summary>
    public static WebApplication UseApiErrorHandling(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.UseStatusCodePages();
        app.UseSecurityHeaders();

        if (!app.Environment.IsDevelopment())
        {
            app.UseHsts();
            app.UseHttpsRedirection();
        }

        return app;
    }

    /// <summary>After any public UI (Swagger): CORS, then who you are, rate limits, then what you may do.</summary>
    public static WebApplication UseApiAccessControl(this WebApplication app)
    {
        app.UseCors(WebCorsPolicy);
        app.UseAuthentication();
        app.UseRateLimiter();
        app.UseAuthorization();
        return app;
    }

    /// <summary>Limits signed-in users by user id, and anonymous callers by IP address.</summary>
    internal static string PartitionKey(HttpContext context)
    {
        var userId = context.User.FindFirstValue("sub");
        return userId is not null
            ? $"user:{userId}"
            : $"ip:{context.Connection.RemoteIpAddress}";
    }

    private static RateLimitPartition<string> PerMinute(HttpContext context, int permitLimit) =>
        RateLimitPartition.GetFixedWindowLimiter(PartitionKey(context), _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = permitLimit,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0,
        });

    private static async ValueTask WriteRateLimitedAsync(
        OnRejectedContext context,
        CancellationToken cancellationToken)
    {
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            context.HttpContext.Response.Headers.RetryAfter =
                ((int)Math.Ceiling(retryAfter.TotalSeconds)).ToString(CultureInfo.InvariantCulture);
        }

        var problems = context.HttpContext.RequestServices.GetRequiredService<IProblemDetailsService>();
        await problems.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = context.HttpContext,
            ProblemDetails =
            {
                Status = StatusCodes.Status429TooManyRequests,
                Title = "Too many requests",
                Detail = "You're doing that too often. Wait a moment and try again.",
            },
        });
    }
}
