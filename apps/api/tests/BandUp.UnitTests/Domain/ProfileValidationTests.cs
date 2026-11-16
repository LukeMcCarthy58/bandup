using BandUp.Application.Profiles;
using BandUp.Domain.Profiles;

namespace BandUp.UnitTests.Domain;

public class ProfileValidationTests
{
    private static readonly HashSet<int> KnownInstruments = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];
    private static readonly HashSet<int> KnownGenres = [1, 2, 3];

    private static ProfileInput Valid() =>
        new("Jade Morgan", "Bassist", 3, ExperienceLevel.Intermediate, Commitment.Regular, [6], [2]);

    private static Dictionary<string, string[]> Validate(ProfileInput input, bool townExists = true) =>
        ProfileValidation.Validate(input, townExists, KnownInstruments, KnownGenres);

    [Fact]
    public void A_complete_profile_is_valid()
    {
        Assert.Empty(Validate(Valid()));
    }

    [Fact]
    public void Whitespace_only_names_are_rejected_after_trimming()
    {
        Assert.Contains("displayName", Validate(Valid() with { DisplayName = "  a   " }).Keys);
    }

    [Fact]
    public void Unknown_town_instrument_or_genre_is_rejected()
    {
        Assert.Contains("townId", Validate(Valid(), townExists: false).Keys);
        Assert.Contains("instrumentIds", Validate(Valid() with { InstrumentIds = [999] }).Keys);
        Assert.Contains("genreIds", Validate(Valid() with { GenreIds = [999] }).Keys);
    }

    [Fact]
    public void Duplicates_count_once_toward_the_limit()
    {
        Assert.Empty(Validate(Valid() with { InstrumentIds = [1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2] }));
        Assert.Contains("instrumentIds", Validate(Valid() with { InstrumentIds = [.. Enumerable.Range(1, 11)] }).Keys);
    }

    [Fact]
    public void Undefined_enum_values_are_rejected()
    {
        var errors = Validate(Valid() with { Level = (ExperienceLevel)42, Commitment = (Commitment)0 });

        Assert.Contains("level", errors.Keys);
        Assert.Contains("commitment", errors.Keys);
    }
}
