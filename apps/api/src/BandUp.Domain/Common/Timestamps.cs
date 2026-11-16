namespace BandUp.Domain.Common;

/// <summary>
/// .NET clocks tick every 100ns but PostgreSQL stores microseconds. Rounding when a time
/// is created means an entity reads back exactly as it was saved.
/// </summary>
public static class Timestamps
{
    public static DateTimeOffset Now() => ToStoragePrecision(DateTimeOffset.UtcNow);

    public static DateTimeOffset ToStoragePrecision(DateTimeOffset value) =>
        value.AddTicks(-(value.Ticks % TimeSpan.TicksPerMicrosecond));
}
