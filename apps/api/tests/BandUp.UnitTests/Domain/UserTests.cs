using BandUp.Domain.Users;

namespace BandUp.UnitTests.Domain;

public class UserTests
{
    [Fact]
    public void Create_links_the_auth_provider_user()
    {
        var user = User.Create("user_2abc");

        Assert.Equal("user_2abc", user.AuthSubject);
        Assert.Equal(7, user.Id.Version);
        Assert.Equal(user.CreatedAt, user.LastSeenAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_rejects_a_blank_subject(string subject)
    {
        Assert.Throws<ArgumentException>(() => User.Create(subject));
    }

    [Fact]
    public void Create_rejects_an_overlong_subject()
    {
        Assert.Throws<ArgumentException>(() => User.Create(new string('x', User.AuthSubjectMaxLength + 1)));
    }

    [Fact]
    public void MarkSeen_only_updates_after_the_granularity_has_passed()
    {
        var user = User.Create("user_2abc");
        var start = user.LastSeenAt;

        Assert.False(user.MarkSeen(start.AddMinutes(5), TimeSpan.FromMinutes(15)));
        Assert.Equal(start, user.LastSeenAt);

        Assert.True(user.MarkSeen(start.AddMinutes(20), TimeSpan.FromMinutes(15)));
        Assert.Equal(start.AddMinutes(20), user.LastSeenAt);
    }
}
