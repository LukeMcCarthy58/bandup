namespace BandUp.Domain.Profiles;

/// <summary>Limits for musician profiles, shared by the domain, API validation and tests.</summary>
public static class ProfileRules
{
    public const int DisplayNameMinLength = 2;
    public const int DisplayNameMaxLength = 60;
    public const int BioMaxLength = 500;
    public const int MaxInstruments = 10;
    public const int MaxGenres = 10;
}
