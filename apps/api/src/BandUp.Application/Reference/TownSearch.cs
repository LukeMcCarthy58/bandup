namespace BandUp.Application.Reference;

/// <summary>Rules for the town autocomplete, shared by the endpoint and its tests.</summary>
public static class TownSearch
{
    public const int MinQueryLength = 2;
    public const int MaxQueryLength = 50;
    public const int DefaultLimit = 10;
    public const int MaxLimit = 20;

    /// <summary>Clamps a requested page size to 1..MaxLimit, defaulting when absent.</summary>
    public static int ClampLimit(int? requested) =>
        requested is null ? DefaultLimit : Math.Clamp(requested.Value, 1, MaxLimit);
}
