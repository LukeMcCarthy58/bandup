using BandUp.Application.Reference;
using BandUp.Domain.Reference;
using BandUp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BandUp.Infrastructure.Reference;

internal sealed class ReferenceDataQueries(BandUpDbContext db) : IReferenceDataQueries
{
    public async Task<IReadOnlyList<ReferenceItem>> GetInstrumentsAsync(CancellationToken cancellationToken) =>
        // Curated order (vocals and guitars first), not alphabetical.
        await db.Set<Instrument>()
            .AsNoTracking()
            .OrderBy(i => i.Id)
            .Select(i => new ReferenceItem(i.Id, i.Name, i.Slug))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ReferenceItem>> GetGenresAsync(CancellationToken cancellationToken)
    {
        var genres = await db.Set<Genre>()
            .AsNoTracking()
            .Select(g => new ReferenceItem(g.Id, g.Name, g.Slug))
            .ToListAsync(cancellationToken);

        // Sorted here, not in SQL, so the order is identical whatever the database collation.
        return [.. genres.OrderBy(g => g.Name, StringComparer.OrdinalIgnoreCase)];
    }

    public async Task<IReadOnlyList<TownSummary>> SearchTownsAsync(
        string query,
        int limit,
        CancellationToken cancellationToken)
    {
        var term = Town.Normalize(query);

        return await db.Set<Town>()
            .AsNoTracking()
            .Where(t => t.SearchName.StartsWith(term))
            // Shortest match first, so "york" ranks York above Yorkshire-somewhere.
            .OrderBy(t => t.SearchName.Length)
            .ThenBy(t => t.SearchName)
            .Take(limit)
            .Select(t => new TownSummary(t.Id, t.Name, t.Region, t.Country))
            .ToListAsync(cancellationToken);
    }
}
