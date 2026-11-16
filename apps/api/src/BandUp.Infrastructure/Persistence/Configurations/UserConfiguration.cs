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
    }
}
