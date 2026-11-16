using BandUp.Application.Reference;
using BandUp.Domain.Reference;

namespace BandUp.UnitTests.Domain;

public class TownTests
{
    [Theory]
    [InlineData("Manchester", "manchester")]
    [InlineData("  Newcastle   upon  Tyne ", "newcastle upon tyne")]
    [InlineData("STOKE-ON-TRENT", "stoke-on-trent")]
    [InlineData("St Albans\t", "st albans")]
    [InlineData("", "")]
    public void Normalize_trims_collapses_spaces_and_lower_cases(string input, string expected)
    {
        Assert.Equal(expected, Town.Normalize(input));
    }

    [Theory]
    [InlineData(null, TownSearch.DefaultLimit)]
    [InlineData(5, 5)]
    [InlineData(0, 1)]
    [InlineData(-3, 1)]
    [InlineData(500, TownSearch.MaxLimit)]
    public void ClampLimit_keeps_page_size_in_range(int? requested, int expected)
    {
        Assert.Equal(expected, TownSearch.ClampLimit(requested));
    }
}
