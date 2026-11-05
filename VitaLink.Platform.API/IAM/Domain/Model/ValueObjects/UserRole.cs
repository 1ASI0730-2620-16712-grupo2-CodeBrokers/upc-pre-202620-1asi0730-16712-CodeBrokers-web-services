namespace VitaLink.Platform.API.IAM.Domain.Model.ValueObjects;

/// <summary>
///     Role a user operates under. It decides which views and endpoints the user can access.
/// </summary>
/// <remarks>
///     Stored as <c>OLDER_ADULT</c>, <c>FAMILY_CAREGIVER</c> or <c>CARE_PROVIDER</c>,
///     matching the <c>iam.users</c> check constraint.
/// </remarks>
public enum UserRole
{
    /// <summary>Older adult whose health is monitored.</summary>
    OlderAdult,

    /// <summary>Family member who cares for an older adult.</summary>
    FamilyCaregiver,

    /// <summary>Health professional who attends alerts.</summary>
    CareProvider
}
