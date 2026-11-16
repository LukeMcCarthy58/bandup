namespace BandUp.Domain.Reference;

/// <summary>An instrument (or role, like vocals or DJ) a musician plays or an advert wants.</summary>
public sealed class Instrument
{
    public int Id { get; init; }

    public required string Name { get; init; }

    /// <summary>URL- and filter-friendly identifier, e.g. "bass-guitar".</summary>
    public required string Slug { get; init; }
}
