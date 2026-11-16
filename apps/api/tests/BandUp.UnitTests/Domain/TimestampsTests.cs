using BandUp.Domain.Common;

namespace BandUp.UnitTests.Domain;

public class TimestampsTests
{
    [Fact]
    public void Rounds_down_to_whole_microseconds()
    {
        var value = new DateTimeOffset(2026, 11, 16, 16, 46, 54, TimeSpan.Zero).AddTicks(908_515);

        var stored = Timestamps.ToStoragePrecision(value);

        Assert.Equal(new DateTimeOffset(2026, 11, 16, 16, 46, 54, TimeSpan.Zero).AddTicks(908_510), stored);
    }

    [Fact]
    public void New_entities_have_storage_precision_timestamps()
    {
        Assert.Equal(0, Timestamps.Now().Ticks % TimeSpan.TicksPerMicrosecond);
    }
}
