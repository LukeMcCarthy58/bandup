using BandUp.Domain.Reference;
using BandUp.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BandUp.Infrastructure.Persistence.Configurations;

internal sealed class InstrumentConfiguration : IEntityTypeConfiguration<Instrument>
{
    public void Configure(EntityTypeBuilder<Instrument> builder)
    {
        builder.ToTable("Instruments");
        builder.Property(i => i.Id).ValueGeneratedNever();
        builder.Property(i => i.Name).HasMaxLength(60);
        builder.Property(i => i.Slug).HasMaxLength(60);
        builder.HasIndex(i => i.Slug).IsUnique();
        builder.HasData(ReferenceSeed.Instruments);
    }
}

internal sealed class GenreConfiguration : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.ToTable("Genres");
        builder.Property(g => g.Id).ValueGeneratedNever();
        builder.Property(g => g.Name).HasMaxLength(60);
        builder.Property(g => g.Slug).HasMaxLength(60);
        builder.HasIndex(g => g.Slug).IsUnique();
        builder.HasData(ReferenceSeed.Genres);
    }
}

internal sealed class TownConfiguration : IEntityTypeConfiguration<Town>
{
    public void Configure(EntityTypeBuilder<Town> builder)
    {
        builder.ToTable("Towns");
        builder.Property(t => t.Id).ValueGeneratedNever();
        builder.Property(t => t.Name).HasMaxLength(100);
        builder.Property(t => t.Region).HasMaxLength(100);
        builder.Property(t => t.Country).HasMaxLength(50);
        builder.Property(t => t.SearchName).HasMaxLength(100);

        // Prefix search ("man" → Manchester) uses this index via LIKE 'man%'.
        builder.HasIndex(t => t.SearchName).HasOperators("text_pattern_ops");

        // Bounding-box distance search (ADR 0008) filters on these.
        builder.HasIndex(t => new { t.Latitude, t.Longitude });

        builder.HasData(ReferenceSeed.Towns);
    }
}
