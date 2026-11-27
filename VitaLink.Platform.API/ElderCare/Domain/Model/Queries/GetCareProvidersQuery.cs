namespace VitaLink.Platform.API.ElderCare.Domain.Model.Queries;

/// <summary>
///     Query to list the active care providers.
/// </summary>
/// <param name="ClinicId">Only the providers of this clinic, when given.</param>
/// <param name="OnlyIndependent">Only the providers without a clinic.</param>
public sealed record GetCareProvidersQuery(
    Guid? ClinicId,
    bool OnlyIndependent);
