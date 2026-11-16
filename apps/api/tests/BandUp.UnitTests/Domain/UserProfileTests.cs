using BandUp.Domain.Profiles;
using BandUp.Domain.Users;

namespace BandUp.UnitTests.Domain;

public class UserProfileTests
{
    private static User UserWithProfile(int[] instruments, int[]? genres = null)
    {
        var user = User.Create("user_1");
        user.UpdateProfile("Jade Morgan", "Bassist", 3, ExperienceLevel.Intermediate, Commitment.Regular, instruments, genres ?? [2]);
        return user;
    }

    [Fact]
    public void A_new_account_has_no_profile()
    {
        Assert.False(User.Create("user_1").HasProfile);
    }

    [Fact]
    public void UpdateProfile_sets_the_profile_and_tidies_text()
    {
        var user = User.Create("user_1");

        user.UpdateProfile("  Jade   Morgan ", "  Bassist  ", 3, ExperienceLevel.Advanced, Commitment.Serious, [6, 1], [2]);

        Assert.True(user.HasProfile);
        Assert.Equal("Jade Morgan", user.DisplayName);
        Assert.Equal("Bassist", user.Bio);
        Assert.Equal(3, user.TownId);
        Assert.Equal(ExperienceLevel.Advanced, user.Level);
        Assert.Equal(Commitment.Serious, user.Commitment);
        Assert.NotNull(user.ProfileUpdatedAt);
    }

    [Fact]
    public void Instruments_keep_the_musicians_order_and_drop_duplicates()
    {
        var user = UserWithProfile([6, 1, 6, 4]);

        Assert.Equal(new[] { 6, 1, 4 }, user.Instruments.OrderBy(i => i.Position).Select(i => i.InstrumentId));
    }

    [Fact]
    public void Updating_instruments_reorders_adds_and_removes_in_place()
    {
        var user = UserWithProfile([6, 1, 4]);
        var bass = user.Instruments.Single(i => i.InstrumentId == 6);

        user.UpdateProfile("Jade", null, 3, ExperienceLevel.Intermediate, Commitment.Regular, [1, 6, 8], []);

        Assert.Equal(new[] { 1, 6, 8 }, user.Instruments.OrderBy(i => i.Position).Select(i => i.InstrumentId));
        Assert.Same(bass, user.Instruments.Single(i => i.InstrumentId == 6));
        Assert.Empty(user.Genres);
        Assert.Null(user.Bio);
    }

    [Theory]
    [InlineData("J")]
    [InlineData("   ")]
    public void Rejects_a_too_short_name(string name)
    {
        var user = User.Create("user_1");
        Assert.Throws<ArgumentException>(() =>
            user.UpdateProfile(name, null, 3, ExperienceLevel.Beginner, Commitment.Casual, [1], []));
    }

    [Fact]
    public void Rejects_no_instruments_or_too_many()
    {
        var user = User.Create("user_1");
        Assert.Throws<ArgumentException>(() =>
            user.UpdateProfile("Jade", null, 3, ExperienceLevel.Beginner, Commitment.Casual, [], []));
        Assert.Throws<ArgumentException>(() =>
            user.UpdateProfile("Jade", null, 3, ExperienceLevel.Beginner, Commitment.Casual, [.. Enumerable.Range(1, 11)], []));
    }
}
