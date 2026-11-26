using VitaLink.Platform.API.IAM.Domain.Model.ValueObjects;

namespace VitaLink.Platform.API.IAM.Interfaces.REST.Transform;

/// <summary>
///     Converts roles to and from the text used by the API (<c>CARE_PROVIDER</c>, ...).
/// </summary>
public static class UserRoleText
{
    /// <summary>Role as API text.</summary>
    public static string ToText(UserRole role) => role switch
    {
        UserRole.OlderAdult => "OLDER_ADULT",
        UserRole.FamilyCaregiver => "FAMILY_CAREGIVER",
        UserRole.CareProvider => "CARE_PROVIDER",
        _ => throw new ArgumentOutOfRangeException(nameof(role), role, "User role is not valid.")
    };

    /// <summary>
    ///     Reads a role from API text.
    /// </summary>
    /// <param name="text">Text such as <c>CARE_PROVIDER</c> (case-insensitive).</param>
    /// <param name="role">The parsed role.</param>
    /// <returns><c>true</c> when the text is a known role.</returns>
    public static bool TryParse(string? text, out UserRole role)
    {
        switch (text?.Trim().ToUpperInvariant())
        {
            case "OLDER_ADULT": role = UserRole.OlderAdult; return true;
            case "FAMILY_CAREGIVER": role = UserRole.FamilyCaregiver; return true;
            case "CARE_PROVIDER": role = UserRole.CareProvider; return true;
            default: role = default; return false;
        }
    }
}
