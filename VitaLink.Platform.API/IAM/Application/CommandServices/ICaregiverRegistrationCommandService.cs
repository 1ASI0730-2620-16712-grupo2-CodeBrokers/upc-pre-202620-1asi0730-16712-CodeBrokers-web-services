using VitaLink.Platform.API.IAM.Application.Results;
using VitaLink.Platform.API.IAM.Domain.Model.Commands;
using VitaLink.Platform.API.IAM.Domain.Model.Errors;
using VitaLink.Platform.API.Shared.Application.Model;

namespace VitaLink.Platform.API.IAM.Application.CommandServices;

/// <summary>
///     Registers a family caregiver with the patient and the first measurements.
/// </summary>
public interface ICaregiverRegistrationCommandService
{
    /// <summary>
    ///     Validates everything first and then saves the account, the patient and the measurements in a single
    ///     transaction: if something fails, nothing is saved. The alerts and their emails are raised after saving.
    /// </summary>
    /// <param name="command">Registration data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<Result<CaregiverRegistrationResult, RegisterFamilyCaregiverError>> Handle(
        RegisterFamilyCaregiverCommand command,
        CancellationToken cancellationToken = default);
}
