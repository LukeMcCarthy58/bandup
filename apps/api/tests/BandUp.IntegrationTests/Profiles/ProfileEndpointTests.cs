using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using BandUp.Application.Profiles;

namespace BandUp.IntegrationTests.Profiles;

public sealed class ProfileEndpointTests(BandUpApiFactory factory) : IClassFixture<BandUpApiFactory>, IDisposable
{
    // Matches the API: camelCase, enums as strings.
    private static readonly JsonSerializerOptions Json = new(JsonSerializerOptions.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    private readonly HttpClient _client = factory.CreateClient();

    public void Dispose() => _client.Dispose();

    private static object ValidBody(string name = "Jade Morgan") => new
    {
        displayName = name,
        bio = "Bassist into tight indie grooves.",
        townId = 3, // Manchester (seeded)
        level = "Intermediate",
        commitment = "Regular",
        instrumentIds = new[] { 6, 1 }, // Bass guitar, Vocals
        genreIds = new[] { 2, 3 }, // Indie, Alternative
    };

    [Fact]
    public async Task Profiles_require_sign_in()
    {
        Assert.Equal(HttpStatusCode.Unauthorized, (await _client.GetAsync("/api/v1/me/profile")).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await _client.PutAsJsonAsync("/api/v1/me/profile", ValidBody())).StatusCode);
    }

    [Fact]
    public async Task A_new_user_starts_without_a_profile()
    {
        var token = TestTokens.Create(TestTokens.NewSubject());

        var profile = await SendAsync<ProfileView>(HttpMethod.Get, "/api/v1/me/profile", token);

        Assert.False(profile.HasProfile);
        Assert.Null(profile.DisplayName);
        Assert.Empty(profile.Instruments);
    }

    [Fact]
    public async Task Saving_a_profile_returns_it_with_names_resolved()
    {
        var token = TestTokens.Create(TestTokens.NewSubject());

        var saved = await SendAsync<ProfileView>(HttpMethod.Put, "/api/v1/me/profile", token, ValidBody());
        var reloaded = await SendAsync<ProfileView>(HttpMethod.Get, "/api/v1/me/profile", token);

        Assert.True(saved.HasProfile);
        Assert.Equal("Jade Morgan", reloaded.DisplayName);
        Assert.Equal("Manchester", reloaded.Town!.Name);
        Assert.Equal(new[] { "Bass guitar", "Vocals" }, reloaded.Instruments.Select(i => i.Name));
        Assert.Equal(new[] { "Alternative", "Indie" }, reloaded.Genres.Select(g => g.Name));
    }

    [Fact]
    public async Task Saving_again_replaces_instruments_and_genres()
    {
        var token = TestTokens.Create(TestTokens.NewSubject());
        await SendAsync<ProfileView>(HttpMethod.Put, "/api/v1/me/profile", token, ValidBody());

        var updated = await SendAsync<ProfileView>(HttpMethod.Put, "/api/v1/me/profile", token, new
        {
            displayName = "Jade M",
            townId = 1,
            level = "Advanced",
            commitment = "Serious",
            instrumentIds = new[] { 8 },
            genreIds = Array.Empty<int>(),
        });

        Assert.Equal("London", updated.Town!.Name);
        Assert.Equal(new[] { "Drums" }, updated.Instruments.Select(i => i.Name));
        Assert.Empty(updated.Genres);
        Assert.Null(updated.Bio);
    }

    [Fact]
    public async Task Invalid_profiles_return_field_errors()
    {
        var token = TestTokens.Create(TestTokens.NewSubject());

        var response = await _client.SendAsync(Request(HttpMethod.Put, "/api/v1/me/profile", token, new
        {
            displayName = "Jade",
            townId = 99999,
            level = "Beginner",
            commitment = "Casual",
            instrumentIds = new[] { 99999 },
        }));
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("townId", body, StringComparison.Ordinal);
        Assert.Contains("instrumentIds", body, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Other_musicians_can_view_a_profile_without_private_details()
    {
        var jade = TestTokens.Create(TestTokens.NewSubject());
        var saved = await SendAsync<ProfileView>(HttpMethod.Put, "/api/v1/me/profile", jade, ValidBody());
        var viewer = TestTokens.Create(TestTokens.NewSubject());

        var response = await _client.SendAsync(Request(HttpMethod.Get, $"/api/v1/users/{saved.UserId}", viewer));
        var json = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Jade Morgan", json, StringComparison.Ordinal);
        Assert.DoesNotContain("authSubject", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("latitude", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("email", json, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Users_without_a_profile_or_unknown_ids_are_not_found()
    {
        var viewer = TestTokens.Create(TestTokens.NewSubject());
        var noProfile = await SendAsync<ProfileView>(HttpMethod.Get, "/api/v1/me/profile", TestTokens.Create(TestTokens.NewSubject()));

        Assert.Equal(HttpStatusCode.NotFound,
            (await _client.SendAsync(Request(HttpMethod.Get, $"/api/v1/users/{noProfile.UserId}", viewer))).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound,
            (await _client.SendAsync(Request(HttpMethod.Get, $"/api/v1/users/{Guid.NewGuid()}", viewer))).StatusCode);
    }

    private static HttpRequestMessage Request(HttpMethod method, string url, string token, object? body = null)
    {
        var request = new HttpRequestMessage(method, url).Authorized(token);
        if (body is not null)
        {
            request.Content = JsonContent.Create(body);
        }

        return request;
    }

    private async Task<T> SendAsync<T>(HttpMethod method, string url, string token, object? body = null)
    {
        var response = await _client.SendAsync(Request(method, url, token, body));
        Assert.True(response.IsSuccessStatusCode, await response.Content.ReadAsStringAsync());
        return (await response.Content.ReadFromJsonAsync<T>(Json))!;
    }
}
