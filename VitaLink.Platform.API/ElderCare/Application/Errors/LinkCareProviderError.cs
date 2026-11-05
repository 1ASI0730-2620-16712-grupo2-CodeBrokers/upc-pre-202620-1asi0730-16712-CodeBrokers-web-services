namespace VitaLink.ElderCare.Application.Errors;

/// <summary>
///     Expected business errors when linking a care provider to an older adult.
/// </summary>
public enum LinkCareProviderError
{
    OlderAdultNotFound,
    CareProviderNotFound,
    CareProviderInactive,
    UnexpectedError
}