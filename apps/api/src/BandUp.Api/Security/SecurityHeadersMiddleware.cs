namespace BandUp.Api.Security;

internal static class SecurityHeadersMiddleware
{
    /// <summary>
    /// Headers for a JSON API: don't sniff types, never render in a frame or as a page,
    /// don't leak the referrer, and don't cache API responses (they're per-user).
    /// </summary>
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app) =>
        app.Use(async (context, next) =>
        {
            context.Response.OnStarting(() =>
            {
                var headers = context.Response.Headers;
                headers.XContentTypeOptions = "nosniff";
                headers.XFrameOptions = "DENY";
                headers["Referrer-Policy"] = "no-referrer";

                // Swagger UI (Development only) is a web page and needs scripts and styles.
                if (!context.Request.Path.StartsWithSegments("/swagger"))
                {
                    headers.ContentSecurityPolicy = "default-src 'none'; frame-ancestors 'none'";
                }

                if (context.Request.Path.StartsWithSegments("/api") && !headers.ContainsKey("Cache-Control"))
                {
                    headers.CacheControl = "no-store";
                }

                return Task.CompletedTask;
            });

            await next(context);
        });
}
