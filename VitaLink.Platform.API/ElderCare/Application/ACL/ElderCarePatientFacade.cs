using VitaLink.Platform.API.ElderCare.Domain.Model.Aggregates;
using VitaLink.Platform.API.ElderCare.Domain.Model.ValueObjects;
using VitaLink.Platform.API.ElderCare.Domain.Repositories;
using VitaLink.Platform.API.ElderCare.Interfaces.ACL;
using VitaLink.Platform.API.Shared.Domain.Repositories;

namespace VitaLink.Platform.API.ElderCare.Application.ACL;

/// <summary>
///     Registers the patient of a family caregiver for other bounded contexts.
/// </summary>
public class ElderCarePatientFacade(
    IOlderAdultRepository olderAdultRepository,
    ICareProviderRepository careProviderRepository,
    IFamilyCaregiverLinkRepository familyCaregiverLinkRepository,
    IUnitOfWork unitOfWork) : IElderCarePatientFacade
{
    private const int NameMaxLength = 100;
    private const int StreetMaxLength = 160;
    private const int DistrictMaxLength = 80;
    private const int CityMaxLength = 80;

    public async Task<PatientRegistrationCheck> CheckAsync(
        PatientRegistrationData patient,
        Guid careProviderId,
        CancellationToken cancellationToken = default)
    {
        var issues = new List<PatientFieldIssue>();

        CheckText(issues, "patient.firstName", patient.FirstName, NameMaxLength, true);
        CheckText(issues, "patient.lastName", patient.LastName, NameMaxLength, true);

        if (patient.BirthDate is null
            || patient.BirthDate == default(DateOnly)
            || patient.BirthDate > DateOnly.FromDateTime(DateTime.UtcNow))
            issues.Add(new PatientFieldIssue("patient.birthDate", PatientIssueKind.InvalidBirthDate));

        CheckText(issues, "patient.street", patient.Street, StreetMaxLength, false);
        CheckText(issues, "patient.district", patient.District, DistrictMaxLength, false);
        CheckText(issues, "patient.city", patient.City, CityMaxLength, false);

        var status = CareProviderStatus.NotFound;

        if (careProviderId != Guid.Empty)
        {
            var careProvider = await careProviderRepository.FindByIdAsync(
                new CareProviderId(careProviderId),
                cancellationToken);

            if (careProvider is not null)
                status = careProvider.IsActive
                    ? CareProviderStatus.Active
                    : CareProviderStatus.Inactive;
        }

        return new PatientRegistrationCheck(issues, status);
    }

    public async Task<Guid> RegisterAsync(
        Guid caregiverUserId,
        string caregiverFirstName,
        string caregiverLastName,
        PatientRegistrationData patient,
        Guid careProviderId,
        CancellationToken cancellationToken = default)
    {
        var hasAddress = !string.IsNullOrWhiteSpace(patient.Street)
                         || !string.IsNullOrWhiteSpace(patient.District)
                         || !string.IsNullOrWhiteSpace(patient.City);

        var olderAdult = new OlderAdult(
            new FullName(patient.FirstName!, patient.LastName!),
            patient.BirthDate!.Value,
            hasAddress
                ? new Address(patient.Street, patient.District, patient.City)
                : null);

        olderAdult.LinkCareProvider(careProviderId);

        await olderAdultRepository.AddAsync(olderAdult, cancellationToken);

        await familyCaregiverLinkRepository.AddAsync(
            new FamilyCaregiverLink(
                caregiverUserId,
                olderAdult.Id.Value,
                caregiverFirstName,
                caregiverLastName),
            cancellationToken);

        await unitOfWork.CompleteAsync(cancellationToken);

        return olderAdult.Id.Value;
    }

    private static void CheckText(
        List<PatientFieldIssue> issues,
        string field,
        string? value,
        int maxLength,
        bool required)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            if (required)
                issues.Add(new PatientFieldIssue(field, PatientIssueKind.Required));

            return;
        }

        if (value.Trim().Length > maxLength)
            issues.Add(new PatientFieldIssue(field, PatientIssueKind.TooLong, maxLength));
    }
}
