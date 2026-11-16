using BandUp.Application.Reference;
using BandUp.Domain.Profiles;

namespace BandUp.Application.Profiles;

/// <summary>What a musician submits for their profile.</summary>
public sealed record ProfileInput(
    string DisplayName,
    string? Bio,
    int TownId,
    ExperienceLevel Level,
    Commitment Commitment,
    IReadOnlyList<int> InstrumentIds,
    IReadOnlyList<int> GenreIds);

/// <summary>
/// A musician's profile as other people see it. Never includes email, exact location or
/// coordinates: only the town's name and region.
/// </summary>
public sealed record ProfileView(
    Guid UserId,
    bool HasProfile,
    string? DisplayName,
    string? Bio,
    TownSummary? Town,
    ExperienceLevel? Level,
    Commitment? Commitment,
    IReadOnlyList<ReferenceItem> Instruments,
    IReadOnlyList<ReferenceItem> Genres,
    DateTimeOffset MemberSince);

/// <summary>Either the saved profile, or field errors keyed by the request's JSON names.</summary>
public sealed record ProfileUpdateResult(ProfileView? Profile, IReadOnlyDictionary<string, string[]> Errors)
{
    public bool Succeeded => Profile is not null;

    public static ProfileUpdateResult Saved(ProfileView profile) => new(profile, new Dictionary<string, string[]>());

    public static ProfileUpdateResult Invalid(IReadOnlyDictionary<string, string[]> errors) => new(null, errors);
}

public interface IProfiles
{
    /// <summary>The signed-in musician's own profile (empty until they set it up).</summary>
    Task<ProfileView> GetMineAsync(string authSubject, CancellationToken cancellationToken);

    Task<ProfileUpdateResult> UpdateMineAsync(string authSubject, ProfileInput input, CancellationToken cancellationToken);

    /// <summary>Another musician's profile, or null if they don't exist or haven't set one up.</summary>
    Task<ProfileView?> GetPublicAsync(Guid userId, CancellationToken cancellationToken);
}
