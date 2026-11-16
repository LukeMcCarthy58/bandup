using BandUp.Application.Users;
using BandUp.Domain.Users;
using BandUp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BandUp.Infrastructure.Users;

internal sealed class UserAccounts(BandUpDbContext db, TimeProvider clock) : IUserAccounts
{
    private static readonly TimeSpan SeenGranularity = TimeSpan.FromMinutes(15);

    public async Task<CurrentUser> GetOrCreateAsync(string authSubject, CancellationToken cancellationToken)
    {
        var existing = await db.Set<User>().SingleOrDefaultAsync(u => u.AuthSubject == authSubject, cancellationToken);
        if (existing is not null)
        {
            if (existing.MarkSeen(clock.GetUtcNow(), SeenGranularity))
            {
                await db.SaveChangesAsync(cancellationToken);
            }

            return ToCurrentUser(existing);
        }

        var user = User.Create(authSubject);
        db.Add(user);
        try
        {
            await db.SaveChangesAsync(cancellationToken);
            return ToCurrentUser(user);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            // Another request created this user at the same moment (e.g. two tabs). Use theirs.
            db.Entry(user).State = EntityState.Detached;
            var created = await db.Set<User>().AsNoTracking()
                .SingleAsync(u => u.AuthSubject == authSubject, cancellationToken);
            return ToCurrentUser(created);
        }
    }

    private static CurrentUser ToCurrentUser(User user) => new(user.Id, user.CreatedAt);
}
