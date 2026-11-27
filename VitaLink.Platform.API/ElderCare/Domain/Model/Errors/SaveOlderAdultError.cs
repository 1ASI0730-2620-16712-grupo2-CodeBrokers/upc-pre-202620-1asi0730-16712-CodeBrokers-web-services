namespace VitaLink.Platform.API.ElderCare.Domain.Model.Errors;

/// <summary>
///     Reasons why an older adult cannot be created or updated.
/// </summary>
public abstract record SaveOlderAdultError
{
    /// <summary>One or more fields are missing or not valid.</summary>
    public sealed record InvalidFields(IReadOnlyList<FieldError> Fields) : SaveOlderAdultError;

    /// <summary>The older adult does not exist.</summary>
    public sealed record OlderAdultNotFound : SaveOlderAdultError;

    /// <summary>The care provider does not exist.</summary>
    public sealed record CareProviderNotFound : SaveOlderAdultError;

    /// <summary>The care provider is not active.</summary>
    public sealed record CareProviderInactive : SaveOlderAdultError;

    /// <summary>Unexpected failure.</summary>
    public sealed record Unexpected : SaveOlderAdultError;
}

/// <summary>
///     A field of the request that is not valid.
/// </summary>
/// <param name="Field">Name of the field, as it appears in the request body.</param>
/// <param name="Code">Why the field is not valid.</param>
/// <param name="Limit">Maximum length, when <paramref name="Code"/> is <see cref="FieldErrorCode.TooLong"/>.</param>
public sealed record FieldError(
    string Field,
    FieldErrorCode Code,
    int? Limit = null);

/// <summary>
///     Why a field is not valid.
/// </summary>
public enum FieldErrorCode
{
    /// <summary>The field is required.</summary>
    Required,

    /// <summary>The field is required to activate the monitoring.</summary>
    RequiredToActivate,

    /// <summary>The text is longer than the allowed limit.</summary>
    TooLong,

    /// <summary>The birth date is missing or in the future.</summary>
    InvalidBirthDate,

    /// <summary>The phone number does not have a valid format.</summary>
    InvalidPhone
}
