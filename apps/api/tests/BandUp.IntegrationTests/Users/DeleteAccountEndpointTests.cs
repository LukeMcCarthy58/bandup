using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using BandUp.Application.Profiles;
using BandUp.Application.Users;

namespace BandUp.IntegrationTests.Users;

public sealed class DeleteAccountEndpointTests(BandUpApiFactory factory) : IClassFixture<BandUpApiFactory>, IDisposable
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerOptions.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    private readonly HttpClient _client = factory.CreateClient();

    public void Dispose() => _client.Dispose();

    [Fact]
    public async Task Requires_sign_in()
    {
        var response = await _client.DeleteAsync("/api/v1/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Deletes_the_account_and_profile()
    {
        var token = TestTokens.Create(TestTokens.NewSubject());
        var before = await SendAsync<CurrentUser>(HttpMethod.Get, "/api/v1/me", token);
        await SendAsync<ProfileView>(HttpMethod.Put, "/api/v1/me/profile", token, ValidProfile());

        var deleted = await SendAsync(HttpMethod.Delete, "/api/v1/me", token);

        Assert.Equal(HttpStatusCode.NoContent, deleted.StatusCode);
        // Their public profile is gone.
        var publicView = await SendAsync(HttpMethod.Get, $"/api/v1/users/{before.Id}", token);
        Assert.Equal(HttpStatusCode.NotFound, publicView.StatusCode);
        // Signing in again (same Clerk user) starts a brand-new, empty account.
        var after = await SendAsync<CurrentUser>(HttpMethod.Get, "/api/v1/me", token);
        Assert.NotEqual(before.Id, after.Id);
        var profile = await SendAsync<ProfileView>(HttpMethod.Get, "/api/v1/me/profile", token);
        Assert.False(profile.HasProfile);
    }

    [Fact]
    public async Task Only_deletes_the_callers_own_account()
    {
        var alice = TestTokens.Create(TestTokens.NewSubject());
        var bob = TestTokens.Create(TestTokens.NewSubject());
        var bobsProfile = await SendAsync<ProfileView>(HttpMethod.Put, "/api/v1/me/profile", bob, ValidProfile("Bob Smith"));
        await SendAsync<CurrentUser>(HttpMethod.Get, "/api/v1/me", alice);

        await SendAsync(HttpMethod.Delete, "/api/v1/me", alice);

        var stillThere = await SendAsync<ProfileView>(HttpMethod.Get, $"/api/v1/users/{bobsProfile.UserId}", alice);
        Assert.Equal("Bob Smith", stillThere.DisplayName);
    }

    [Fact]
    public async Task Deleting_twice_is_safe()
    {
        var token = TestTokens.Create(TestTokens.NewSubject());
        await SendAsync<CurrentUser>(HttpMethod.Get, "/api/v1/me", token);

        var first = await SendAsync(HttpMethod.Delete, "/api/v1/me", token);
        var second = await SendAsync(HttpMethod.Delete, "/api/v1/me", token);

        Assert.Equal(HttpStatusCode.NoContent, first.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, second.StatusCode);
    }

    private static object ValidProfile(string name = "Jade Morgan") => new
    {
        displayName = name,
        townId = 3,
        level = "Intermediate",
        commitment = "Regular",
        instrumentIds = new[] { 6 },
        genreIds = new[] { 2 },
    };

    private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string url, string token, object? body = null)
    {
        var request = new HttpRequestMessage(method, url).Authorized(token);
        if (body is not null)
        {
            request.Content = JsonContent.Create(body, options: Json);
        }

        return await _client.SendAsync(request);
    }

    private async Task<T> SendAsync<T>(HttpMethod method, string url, string token, object? body = null)
    {
        var response = await SendAsync(method, url, token, body);
        Assert.True(response.IsSuccessStatusCode, $"{method} {url} returned {(int)response.StatusCode}");
        return (await response.Content.ReadFromJsonAsync<T>(Json))!;
    }
}
