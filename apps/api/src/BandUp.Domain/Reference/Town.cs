using System.Text;

namespace BandUp.Domain.Reference;

/// <summary>
/// A town or area. Users and bands pick a town rather than an address, so BandUp only
/// ever knows roughly where someone is. The centroid is used for distance search and is
/// never returned by the API.
/// </summary>
public sealed class Town
{
    public int Id { get; init; }

    public required string Name { get; init; }

    /// <summary>County or region, to tell apart towns with the same name.</summary>
    public required string Region { get; init; }

    public required string Country { get; init; }

    public double Latitude { get; init; }

    public double Longitude { get; init; }

    /// <summary>Lower-cased, trimmed name used for fast prefix search.</summary>
    public string SearchName { get; init; } = string.Empty;

    /// <summary>Normalises a name or search term: trimmed, single-spaced, lower-case.</summary>
    public static string Normalize(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var builder = new StringBuilder(value.Length);
        var lastWasSpace = false;
        foreach (var c in value.Trim())
        {
            if (char.IsWhiteSpace(c))
            {
                if (!lastWasSpace)
                {
                    builder.Append(' ');
                }

                lastWasSpace = true;
                continue;
            }

            builder.Append(char.ToLowerInvariant(c));
            lastWasSpace = false;
        }

        return builder.ToString();
    }
}
