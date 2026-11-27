using VitaLink.Platform.API.ElderCare.Domain.Model.Aggregates;
using VitaLink.Platform.API.ElderCare.Domain.Model.ValueObjects;
using VitaLink.Platform.API.ElderCare.Domain.Repositories;
using VitaLink.Platform.API.ElderCare.Domain.Services;
using VitaLink.Platform.API.ElderCare.Interfaces.ACL;

namespace VitaLink.Platform.API.ElderCare.Application.ACL;

/// <inheritdoc />
public class ElderCareContextFacade(
    ICareProviderRepository careProviderRepository,
    IClinicRepository clinicRepository,
    ILicenseValidationService licenseValidationService) : IElderCareContextFacade
{
    /// <inheritdoc />
    public async Task<CareProviderProfileOutcome> AddCareProviderProfileAsync(
        Guid userId,
        string firstName,
        string lastName,
        string specialty,
        string licenseNumber,
        Guid? clinicId,
        CancellationToken cancellationToken = default)
    {
        FullName fullName;
        Specialty providerSpecialty;
        try
        {
            fullName = new FullName(firstName, lastName);
            providerSpecialty = new Specialty(specialty);
        }
        catch (ArgumentException)
        {
            return CareProviderProfileOutcome.InvalidData;
        }

        if (await careProviderRepository.FindByUserIdAsync(userId, cancellationToken) is not null)
            return CareProviderProfileOutcome.AlreadyExists;

        var license = await licenseValidationService.ValidateAsync(licenseNumber, cancellationToken);
        if (license is LicenseValidationResult.InvalidFormat or LicenseValidationResult.NotRegistered)
            return CareProviderProfileOutcome.InvalidLicense;
        if (license == LicenseValidationResult.AlreadyInUse)
            return CareProviderProfileOutcome.LicenseAlreadyInUse;

        if (clinicId is not null
            && await clinicRepository.FindByIdAsync(new ClinicId(clinicId.Value), cancellationToken) is null)
            return CareProviderProfileOutcome.ClinicNotFound;

        CareProvider careProvider;
        try
        {
            careProvider = new CareProvider(userId, fullName, providerSpecialty, licenseNumber, clinicId);
        }
        catch (ArgumentException)
        {
            return CareProviderProfileOutcome.InvalidData;
        }

        await careProviderRepository.AddAsync(careProvider, cancellationToken);
        return CareProviderProfileOutcome.Added;
    }
}
