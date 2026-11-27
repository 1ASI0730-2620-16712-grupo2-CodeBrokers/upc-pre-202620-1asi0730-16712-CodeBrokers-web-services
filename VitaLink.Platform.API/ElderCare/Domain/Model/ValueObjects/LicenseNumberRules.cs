using System.Text.RegularExpressions;

namespace VitaLink.Platform.API.ElderCare.Domain.Model.ValueObjects;

/// <summary>
///     Format rules of a professional license number: 4 to 10 digits.
/// </summary>
public static class LicenseNumberRules
{
    public const int MinLength = 4;
    public const int MaxLength = 10;

    private static readonly Regex Pattern = new("^[0-9]{4,10}$", RegexOptions.Compiled);

    /// <summary>
    ///     Removes the surrounding spaces of a license number.
    /// </summary>
    public static string? Normalize(string? value)
    {
        return value?.Trim();
    }

    /// <summary>
    ///     Indicates whether the value has 4 to 10 digits.
    /// </summary>
    public static bool IsValidFormat(string? value)
    {
        var normalized = Normalize(value);

        return !string.IsNullOrEmpty(normalized) && Pattern.IsMatch(normalized);
    }
}
