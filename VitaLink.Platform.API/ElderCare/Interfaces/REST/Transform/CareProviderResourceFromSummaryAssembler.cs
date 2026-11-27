using VitaLink.Platform.API.ElderCare.Application.Results;
using VitaLink.Platform.API.ElderCare.Interfaces.REST.Resources;

namespace VitaLink.Platform.API.ElderCare.Interfaces.REST.Transform;

/// <summary>
///     Converts a care provider summary into its REST resource.
/// </summary>
public static class CareProviderResourceFromSummaryAssembler
{
    public static CareProviderResource ToResourceFromSummary(CareProviderSummary summary)
    {
        var provider = summary.Provider;

        return new CareProviderResource(
            provider.Id.Value,
            provider.FullName.FirstName,
            provider.FullName.LastName,
            provider.Specialty.Value,
            provider.ClinicId,
            summary.ClinicName);
    }
}
