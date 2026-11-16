using BandUp.Application.Profiles;
using BandUp.Application.Reference;
using BandUp.Application.Users;
using BandUp.Domain.Reference;
using BandUp.Domain.Users;
using BandUp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BandUp.Infrastructure.Profiles;

internal sealed class ProfileService(BandUpDbContext db, IUserAccounts accounts) : IProfiles
{
    public async Task<ProfileView> GetMineAsync(string authSubject, CancellationToken cancellationToken)
    {
        var me = await accounts.GetOrCreateAsync(authSubject, cancellationToken);
        return await LoadViewAsync(me.Id, cancellationToken)
            ?? throw new InvalidOperationException("Account vanished after creation.");
    }

    public async Task<ProfileUpdateResult> UpdateMineAsync(
        string authSubject,
        ProfileInput input,
        CancellationToken cancellationToken)
    {
        var errors = await ValidateAsync(input, cancellationToken);
        if (errors.Count > 0)
        {
            return ProfileUpdateResult.Invalid(errors);
        }

        var me = await accounts.GetOrCreateAsync(authSubject, cancellationToken);
        var user = await db.Set<User>()
            .Include(u => u.Instruments)
            .Include(u => u.Genres)
            .SingleAsync(u => u.Id == me.Id, cancellationToken);

        user.UpdateProfile(
            input.DisplayName,
            input.Bio,
            input.TownId,
            input.Level,
            input.Commitment,
            input.InstrumentIds,
            input.GenreIds);
        await db.SaveChangesAsync(cancellationToken);

        return ProfileUpdateResult.Saved((await LoadViewAsync(user.Id, cancellationToken))!);
    }

    public async Task<ProfileView?> GetPublicAsync(Guid userId, CancellationToken cancellationToken)
    {
        var view = await LoadViewAsync(userId, cancellationToken);
        return view is { HasProfile: true } ? view : null;
    }

    private async Task<Dictionary<string, string[]>> ValidateAsync(ProfileInput input, CancellationToken ct)
    {
        var townExists = await db.Set<Town>().AnyAsync(t => t.Id == input.TownId, ct);
        var instrumentIds = input.InstrumentIds.Distinct().ToList();
        var genreIds = input.GenreIds.Distinct().ToList();

        var knownInstruments = await db.Set<Instrument>()
            .Where(i => instrumentIds.Contains(i.Id))
            .Select(i => i.Id)
            .ToListAsync(ct);
        var knownGenres = await db.Set<Genre>()
            .Where(g => genreIds.Contains(g.Id))
            .Select(g => g.Id)
            .ToListAsync(ct);

        return ProfileValidation.Validate(input, townExists, knownInstruments.ToHashSet(), knownGenres.ToHashSet());
    }

    private async Task<ProfileView?> LoadViewAsync(Guid userId, CancellationToken ct)
    {
        var row = await db.Set<User>()
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new
            {
                u.Id,
                u.DisplayName,
                u.Bio,
                u.TownId,
                u.Level,
                u.Commitment,
                u.CreatedAt,
                InstrumentIds = u.Instruments.OrderBy(i => i.Position).Select(i => i.InstrumentId).ToList(),
                GenreIds = u.Genres.Select(g => g.GenreId).ToList(),
            })
            .SingleOrDefaultAsync(ct);

        if (row is null)
        {
            return null;
        }

        var instruments = await db.Set<Instrument>()
            .AsNoTracking()
            .Where(i => row.InstrumentIds.Contains(i.Id))
            .Select(i => new ReferenceItem(i.Id, i.Name, i.Slug))
            .ToListAsync(ct);
        var genres = await db.Set<Genre>()
            .AsNoTracking()
            .Where(g => row.GenreIds.Contains(g.Id))
            .Select(g => new ReferenceItem(g.Id, g.Name, g.Slug))
            .ToListAsync(ct);
        var town = row.TownId is null
            ? null
            : await db.Set<Town>()
                .AsNoTracking()
                .Where(t => t.Id == row.TownId)
                .Select(t => new TownSummary(t.Id, t.Name, t.Region, t.Country))
                .SingleOrDefaultAsync(ct);

        var hasProfile = row.DisplayName is not null && town is not null && row.InstrumentIds.Count > 0;

        return new ProfileView(
            row.Id,
            hasProfile,
            row.DisplayName,
            row.Bio,
            town,
            row.Level,
            row.Commitment,
            // Keep the musician's instrument order; genres alphabetically.
            [.. row.InstrumentIds.Select(id => instruments.Single(i => i.Id == id))],
            [.. genres.OrderBy(g => g.Name, StringComparer.OrdinalIgnoreCase)],
            row.CreatedAt);
    }
}
