using System.Text.Json.Serialization;
using BandUp.Api.Endpoints;
using BandUp.Api.Security;
using BandUp.Application;
using BandUp.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

// Structured JSON logs outside Development. Tokens and request bodies are never logged.
if (!builder.Environment.IsDevelopment())
{
    builder.Logging.ClearProviders();
    builder.Logging.AddJsonConsole();
}

// JSON contract: enums as readable strings ("Intermediate"), and numbers strictly as
// numbers (so the OpenAPI document, and the generated TypeScript, say "integer").
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.SerializerOptions.NumberHandling = JsonNumberHandling.Strict;
});

builder.Services.AddOpenApi();
builder.Services.AddValidation();
builder.Services.AddApiSecurity(builder.Configuration);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseApiErrorHandling();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "BandUp API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseApiAccessControl();

app.MapHealthEndpoints();

var api = app.MapGroup("/api/v1");
api.MapReferenceDataEndpoints();
api.MapMeEndpoints();
api.MapProfileEndpoints();

app.Run();

/// <summary>Exposed for WebApplicationFactory in integration tests.</summary>
public partial class Program;
