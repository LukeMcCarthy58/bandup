namespace BandUp.Domain.Reference;

/// <summary>A music genre used by profiles, bands and adverts.</summary>
public sealed class Genre
{
    public int Id { get; init; }

    public required string Name { get; init; }

    /// <summary>URL- and filter-friendly identifier, e.g. "drum-and-bass".</summary>
    public required string Slug { get; init; }
}
