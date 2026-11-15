using Microsoft.EntityFrameworkCore;

namespace BandUp.Infrastructure.Persistence;

public sealed class BandUpDbContext(DbContextOptions<BandUpDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Entity configurations (IEntityTypeConfiguration<T>) live next to this file.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BandUpDbContext).Assembly);
    }
}
