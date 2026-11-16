namespace BandUp.Application.Reference;

public sealed record ReferenceItem(int Id, string Name, string Slug);

/// <summary>A town as shown to users. Deliberately has no coordinates.</summary>
public sealed record TownSummary(int Id, string Name, string Region, string Country);

/// <summary>Read-only lookups for instruments, genres and towns.</summary>
public interface IReferenceDataQueries
{
    Task<IReadOnlyList<ReferenceItem>> GetInstrumentsAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<ReferenceItem>> GetGenresAsync(CancellationToken cancellationToken);

    /// <summary>Towns whose name starts with the query, for autocomplete.</summary>
    Task<IReadOnlyList<TownSummary>> SearchTownsAsync(string query, int limit, CancellationToken cancellationToken);
}
