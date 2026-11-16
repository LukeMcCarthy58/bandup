using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BandUp.Application.Reference;

namespace BandUp.IntegrationTests.Reference;

public sealed class ReferenceDataEndpointTests(BandUpApiFactory factory) : IClassFixture<BandUpApiFactory>, IDisposable
{
    private readonly HttpClient _client = factory.CreateClient();

    public void Dispose() => _client.Dispose();

    [Fact]
    public async Task Instruments_are_public_and_in_picker_order()
    {
        var response = await _client.GetAsync("/api/v1/instruments");
        var instruments = await response.Content.ReadFromJsonAsync<List<ReferenceItem>>(JsonSerializerOptions.Web);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Vocals", instruments![0].Name);
        Assert.Contains(instruments, i => i.Slug == "bass-guitar");
        Assert.Equal("public, max-age=3600", response.Headers.CacheControl?.ToString());
    }

    [Fact]
    public async Task Genres_are_public_and_alphabetical()
    {
        var genres = await _client.GetFromJsonAsync<List<ReferenceItem>>("/api/v1/genres", JsonSerializerOptions.Web);

        Assert.NotNull(genres);
        Assert.Contains(genres, g => g.Slug == "drum-and-bass");
        var names = genres.Select(g => g.Name).ToList();
        Assert.Equal(names.Order(StringComparer.OrdinalIgnoreCase), names);
    }

    [Theory]
    [InlineData("man", "Manchester")]
    [InlineData("  NEWCASTLE upon", "Newcastle upon Tyne")]
    [InlineData("york", "York")]
    public async Task Town_search_matches_by_prefix_ignoring_case_and_spacing(string query, string expectedFirst)
    {
        var towns = await _client.GetFromJsonAsync<List<TownSummary>>(
            $"/api/v1/towns?query={Uri.EscapeDataString(query)}", JsonSerializerOptions.Web);

        Assert.Equal(expectedFirst, towns![0].Name);
    }

    [Fact]
    public async Task Town_search_never_returns_coordinates()
    {
        var json = await _client.GetStringAsync("/api/v1/towns?query=london");

        Assert.Contains("London", json, StringComparison.Ordinal);
        Assert.DoesNotContain("latitude", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("longitude", json, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Town_search_caps_the_page_size()
    {
        var towns = await _client.GetFromJsonAsync<List<TownSummary>>("/api/v1/towns?query=st&limit=500", JsonSerializerOptions.Web);

        Assert.InRange(towns!.Count, 1, TownSearch.MaxLimit);
    }

    [Theory]
    [InlineData("/api/v1/towns")]
    [InlineData("/api/v1/towns?query=m")]
    public async Task Town_search_needs_at_least_two_characters(string url)
    {
        var response = await _client.GetAsync(url);
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("\"query\"", body, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Town_search_with_no_matches_returns_an_empty_list()
    {
        var towns = await _client.GetFromJsonAsync<List<TownSummary>>("/api/v1/towns?query=zzqx", JsonSerializerOptions.Web);

        Assert.Empty(towns!);
    }
}
