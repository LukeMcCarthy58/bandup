using BandUp.Domain.Profiles;
using BandUp.Domain.Reference;
using BandUp.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BandUp.Infrastructure.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.Property(u => u.Id).ValueGeneratedNever();
        builder.Property(u => u.AuthSubject).HasMaxLength(User.AuthSubjectMaxLength);

        // One BandUp account per auth-provider user; also makes first-request creation race-safe.
        builder.HasIndex(u => u.AuthSubject).IsUnique();

        builder.Property(u => u.DisplayName).HasMaxLength(ProfileRules.DisplayNameMaxLength);
        builder.Property(u => u.Bio).HasMaxLength(ProfileRules.BioMaxLength);

        // Stored as text so the database is readable and reordering the enum is safe.
        builder.Property(u => u.Level).HasConversion<string>().HasMaxLength(20);
        builder.Property(u => u.Commitment).HasConversion<string>().HasMaxLength(20);

        builder.HasOne<Town>()
            .WithMany()
            .HasForeignKey(u => u.TownId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.Instruments)
            .WithOne()
            .HasForeignKey(i => i.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(u => u.Instruments)
            .HasField("_instruments")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(u => u.Genres)
            .WithOne()
            .HasForeignKey(g => g.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(u => u.Genres)
            .HasField("_genres")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

internal sealed class UserInstrumentConfiguration : IEntityTypeConfiguration<UserInstrument>
{
    public void Configure(EntityTypeBuilder<UserInstrument> builder)
    {
        builder.ToTable("UserInstruments");
        builder.HasKey(i => new { i.UserId, i.InstrumentId });
        builder.HasOne<Instrument>()
            .WithMany()
            .HasForeignKey(i => i.InstrumentId)
            .OnDelete(DeleteBehavior.Restrict);

        // "Musicians who play X" (advert matching, Recommended feed).
        builder.HasIndex(i => i.InstrumentId);
    }
}

internal sealed class UserGenreConfiguration : IEntityTypeConfiguration<UserGenre>
{
    public void Configure(EntityTypeBuilder<UserGenre> builder)
    {
        builder.ToTable("UserGenres");
        builder.HasKey(g => new { g.UserId, g.GenreId });
        builder.HasOne<Genre>()
            .WithMany()
            .HasForeignKey(g => g.GenreId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(g => g.GenreId);
    }
}
