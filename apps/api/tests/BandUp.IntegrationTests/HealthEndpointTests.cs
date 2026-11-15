using System.Net;

namespace BandUp.IntegrationTests;

public class HealthEndpointTests(BandUpApiFactory factory) : IClassFixture<BandUpApiFactory>
{
    [Fact]
    public async Task Liveness_returns_healthy()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/health/live");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Healthy", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task OpenApi_document_is_served_in_development()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/openapi/v1.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Database")]
    public async Task Readiness_reports_database_status()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/health");

        // With a real database configured it must be healthy; without one it must
        // report unhealthy (503) rather than crash.
        var expected = BandUpApiFactory.DatabaseConnectionString is null
            ? HttpStatusCode.ServiceUnavailable
            : HttpStatusCode.OK;
        Assert.Equal(expected, response.StatusCode);
    }
}
