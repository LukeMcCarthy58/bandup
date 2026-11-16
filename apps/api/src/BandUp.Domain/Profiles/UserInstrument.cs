namespace BandUp.Domain.Profiles;

/// <summary>An instrument a musician plays. Position 0 is their main instrument.</summary>
public sealed class UserInstrument
{
    public Guid UserId { get; init; }

    public int InstrumentId { get; init; }

    public int Position { get; set; }
}

/// <summary>A genre a musician is into.</summary>
public sealed class UserGenre
{
    public Guid UserId { get; init; }

    public int GenreId { get; init; }
}
