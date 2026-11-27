using VitaLink.Platform.API.ElderCare.Domain.Model.Aggregates;

namespace VitaLink.Platform.API.ElderCare.Application.Results;

/// <summary>
///     A care provider together with the name of its clinic (null when independent).
/// </summary>
/// <param name="Provider">The care provider.</param>
/// <param name="ClinicName">Name of the clinic the provider works in.</param>
public sealed record CareProviderSummary(
    CareProvider Provider,
    string? ClinicName);
