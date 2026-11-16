using BandUp.Domain.Common;
using BandUp.Domain.Profiles;

namespace BandUp.Domain.Users;

/// <summary>
/// A BandUp account and its musician profile. Identity (passwords, Google sign-in, sessions)
/// lives in the auth provider; this links its user to BandUp's own data.
/// </summary>
public sealed class User : Entity
{
    public const int AuthSubjectMaxLength = 64;

    private readonly List<UserInstrument> _instruments = [];
    private readonly List<UserGenre> _genres = [];

    // For EF Core.
    private User()
    {
    }

    private User(string authSubject)
    {
        AuthSubject = authSubject;
        LastSeenAt = CreatedAt;
    }

    /// <summary>The auth provider's stable user id (the token's "sub" claim), e.g. Clerk's "user_…".</summary>
    public string AuthSubject { get; private set; } = string.Empty;

    public DateTimeOffset LastSeenAt { get; private set; }

    // Profile. Empty until the musician sets it up.

    public string? DisplayName { get; private set; }

    public string? Bio { get; private set; }

    /// <summary>The town or area they're based near. Never an address.</summary>
    public int? TownId { get; private set; }

    public ExperienceLevel? Level { get; private set; }

    public Commitment? Commitment { get; private set; }

    public DateTimeOffset? ProfileUpdatedAt { get; private set; }

    /// <summary>Instruments the musician plays; order by Position (0 is their main one).</summary>
    public IReadOnlyList<UserInstrument> Instruments => _instruments;

    public IReadOnlyList<UserGenre> Genres => _genres;

    /// <summary>A profile is set up once it has the details other musicians search on.</summary>
    public bool HasProfile => DisplayName is not null && TownId is not null && _instruments.Count > 0;

    public static User Create(string authSubject)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(authSubject);
        if (authSubject.Length > AuthSubjectMaxLength)
        {
            throw new ArgumentException($"Auth subject is longer than {AuthSubjectMaxLength} characters.", nameof(authSubject));
        }

        return new User(authSubject);
    }

    /// <summary>Records activity, at most once per <paramref name="granularity"/> to avoid a write per request.</summary>
    public bool MarkSeen(DateTimeOffset now, TimeSpan granularity)
    {
        if (now - LastSeenAt < granularity)
        {
            return false;
        }

        LastSeenAt = Timestamps.ToStoragePrecision(now);
        return true;
    }

    /// <summary>
    /// Replaces the profile. Inputs are expected to be validated (lengths, known ids) by the
    /// application layer; these guards protect the invariants if that's ever bypassed.
    /// </summary>
    public void UpdateProfile(
        string displayName,
        string? bio,
        int townId,
        ExperienceLevel level,
        Commitment commitment,
        IReadOnlyList<int> instrumentIds,
        IReadOnlyList<int> genreIds)
    {
        var name = CollapseWhitespace(displayName);
        if (name.Length is < ProfileRules.DisplayNameMinLength or > ProfileRules.DisplayNameMaxLength)
        {
            throw new ArgumentException("Display name length is out of range.", nameof(displayName));
        }

        var cleanBio = string.IsNullOrWhiteSpace(bio) ? null : bio.Trim();
        if (cleanBio?.Length > ProfileRules.BioMaxLength)
        {
            throw new ArgumentException("Bio is too long.", nameof(bio));
        }

        var instruments = instrumentIds.Distinct().ToList();
        if (instruments.Count is 0 or > ProfileRules.MaxInstruments)
        {
            throw new ArgumentException("A profile needs 1 to 10 instruments.", nameof(instrumentIds));
        }

        var genres = genreIds.Distinct().ToList();
        if (genres.Count > ProfileRules.MaxGenres)
        {
            throw new ArgumentException("A profile can have up to 10 genres.", nameof(genreIds));
        }

        DisplayName = name;
        Bio = cleanBio;
        TownId = townId;
        Level = level;
        Commitment = commitment;
        SyncInstruments(instruments);
        SyncGenres(genres);
        ProfileUpdatedAt = Timestamps.Now();
    }

    // Updates the join rows in place (rather than clear-and-re-add) so EF Core sees
    // precise inserts, deletes and position changes.
    private void SyncInstruments(List<int> instrumentIds)
    {
        _instruments.RemoveAll(i => !instrumentIds.Contains(i.InstrumentId));
        for (var position = 0; position < instrumentIds.Count; position++)
        {
            var id = instrumentIds[position];
            var existing = _instruments.Find(i => i.InstrumentId == id);
            if (existing is null)
            {
                _instruments.Add(new UserInstrument { UserId = Id, InstrumentId = id, Position = position });
            }
            else
            {
                existing.Position = position;
            }
        }
    }

    private void SyncGenres(List<int> genreIds)
    {
        _genres.RemoveAll(g => !genreIds.Contains(g.GenreId));
        foreach (var id in genreIds.Where(id => !_genres.Exists(g => g.GenreId == id)))
        {
            _genres.Add(new UserGenre { UserId = Id, GenreId = id });
        }
    }

    private static string CollapseWhitespace(string value) =>
        string.Join(' ', value.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
}
