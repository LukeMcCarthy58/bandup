using BandUp.Domain.Profiles;

namespace BandUp.Application.Profiles;

/// <summary>
/// Checks a profile against the rules and against what exists (towns, instruments, genres).
/// Error keys match the request's JSON property names so the web form can show them inline.
/// </summary>
public static class ProfileValidation
{
    public static Dictionary<string, string[]> Validate(
        ProfileInput input,
        bool townExists,
        IReadOnlySet<int> knownInstrumentIds,
        IReadOnlySet<int> knownGenreIds)
    {
        ArgumentNullException.ThrowIfNull(input);
        var errors = new Dictionary<string, string[]>();

        var name = string.Join(' ', (input.DisplayName ?? string.Empty).Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
        if (name.Length is < ProfileRules.DisplayNameMinLength or > ProfileRules.DisplayNameMaxLength)
        {
            errors["displayName"] =
                [$"Enter a name between {ProfileRules.DisplayNameMinLength} and {ProfileRules.DisplayNameMaxLength} characters."];
        }

        if (input.Bio?.Trim().Length > ProfileRules.BioMaxLength)
        {
            errors["bio"] = [$"Keep your bio under {ProfileRules.BioMaxLength} characters."];
        }

        if (!townExists)
        {
            errors["townId"] = ["Choose your town from the list."];
        }

        var instruments = input.InstrumentIds.Distinct().ToList();
        if (instruments.Count == 0)
        {
            errors["instrumentIds"] = ["Choose at least one instrument."];
        }
        else if (instruments.Count > ProfileRules.MaxInstruments)
        {
            errors["instrumentIds"] = [$"Choose up to {ProfileRules.MaxInstruments} instruments."];
        }
        else if (instruments.Any(id => !knownInstrumentIds.Contains(id)))
        {
            errors["instrumentIds"] = ["One of those instruments isn't in our list."];
        }

        var genres = input.GenreIds.Distinct().ToList();
        if (genres.Count > ProfileRules.MaxGenres)
        {
            errors["genreIds"] = [$"Choose up to {ProfileRules.MaxGenres} genres."];
        }
        else if (genres.Any(id => !knownGenreIds.Contains(id)))
        {
            errors["genreIds"] = ["One of those genres isn't in our list."];
        }

        if (!Enum.IsDefined(input.Level))
        {
            errors["level"] = ["Choose your level."];
        }

        if (!Enum.IsDefined(input.Commitment))
        {
            errors["commitment"] = ["Choose how much time you can give a band."];
        }

        return errors;
    }
}
