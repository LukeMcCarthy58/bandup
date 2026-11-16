using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BandUp.Application.Users;

namespace BandUp.IntegrationTests.Users;

public sealed class MeEndpointTests(BandUpApiFactory factory) : IClassFixture<BandUpApiFactory>, IDisposable
{
    private readonly HttpClient _client = factory.CreateClient();

    public void Dispose() => _client.Dispose();

    [Fact]
    public async Task Requires_sign_in()
    {
        var response = await _client.GetAsync("/api/v1/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Creates_the_account_on_first_call_and_returns_the_same_one_after()
    {
        var token = TestTokens.Create(TestTokens.NewSubject());

        var first = await GetMeAsync(token);
        var second = await GetMeAsync(token);

        Assert.NotEqual(Guid.Empty, first.Id);
        Assert.Equal(first.Id, second.Id);
        Assert.Equal(first.CreatedAt, second.CreatedAt);
    }

    [Fact]
    public async Task Different_users_get_different_accounts()
    {
        var alice = await GetMeAsync(TestTokens.Create(TestTokens.NewSubject()));
        var bob = await GetMeAsync(TestTokens.Create(TestTokens.NewSubject()));

        Assert.NotEqual(alice.Id, bob.Id);
    }

    [Fact]
    public async Task Simultaneous_first_requests_create_exactly_one_account()
    {
        var token = TestTokens.Create(TestTokens.NewSubject());

        var results = await Task.WhenAll(Enumerable.Range(0, 8).Select(_ => GetMeAsync(token)));

        Assert.Single(results.Select(r => r.Id).Distinct());
    }

    [Fact]
    public async Task Tokens_issued_to_another_site_are_rejected()
    {
        var token = TestTokens.Create(TestTokens.NewSubject(), authorizedParty: "https://evil.example");

        var response = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "/api/v1/me").Authorized(token));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private async Task<CurrentUser> GetMeAsync(string token)
    {
        var response = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "/api/v1/me").Authorized(token));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<CurrentUser>(JsonSerializerOptions.Web))!;
    }
}
