using System.Transactions;
using VitaLink.Platform.API.IAM.Application.CommandServices;
using VitaLink.Platform.API.IAM.Application.Internal.OutboundServices.ACL;
using VitaLink.Platform.API.IAM.Application.Results;
using VitaLink.Platform.API.IAM.Domain.Model.Aggregates;
using VitaLink.Platform.API.IAM.Domain.Model.Commands;
using VitaLink.Platform.API.IAM.Domain.Model.Errors;
using VitaLink.Platform.API.IAM.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Shared.Application.Model;

namespace VitaLink.Platform.API.IAM.Application.Internal.CommandServices;

/// <inheritdoc />
public class CaregiverRegistrationCommandService(
    IUserCommandService userCommandService,
    ExternalCaregiverRegistrationService externalCaregiverRegistrationService)
    : ICaregiverRegistrationCommandService
{
    private const int NameMaxLength = 100;

    /// <inheritdoc />
    public async Task<Result<CaregiverRegistrationResult, RegisterFamilyCaregiverError>> Handle(
        RegisterFamilyCaregiverCommand command,
        CancellationToken cancellationToken = default)
    {
        // 1. Validate everything before saving anything, so all the problems are reported together.
        var errors = ValidateAccount(command);

        errors.AddRange(await externalCaregiverRegistrationService.CheckAsync(
            command.Patient,
            command.CareProviderId,
            command.Measurements,
            cancellationToken));

        if (errors.Count > 0)
            return Fail(new RegisterFamilyCaregiverError.InvalidFields(errors));

        Guid olderAdultId;
        IReadOnlyList<Guid> healthRecordIds;
        User user;

        // 2. Save the account, the patient and the measurements in one transaction. The patient and the
        // measurements reference the account, and EF Core cannot order inserts across bounded contexts, so each
        // part is saved in order inside the same scope. Leaving the scope without completing it rolls back all.
        using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            var signUp = await userCommandService.Handle(
                new SignUpCommand(command.Email!, command.Password!, UserRole.FamilyCaregiver),
                cancellationToken);

            if (signUp is Result<User, SignUpError>.Failure failure)
            {
                return failure.Error == SignUpError.EmailAlreadyRegistered
                    ? Fail(new RegisterFamilyCaregiverError.EmailAlreadyRegistered())
                    : Fail(new RegisterFamilyCaregiverError.Unexpected());
            }

            user = ((Result<User, SignUpError>.Success)signUp).Value;

            olderAdultId = await externalCaregiverRegistrationService.RegisterPatientAsync(
                user.Id.Value,
                command.FirstName!,
                command.LastName!,
                command.Patient,
                command.CareProviderId!.Value,
                cancellationToken);

            healthRecordIds = await externalCaregiverRegistrationService.SaveMeasurementsAsync(
                olderAdultId,
                user.Id.Value,
                command.Measurements,
                cancellationToken);

            transaction.Complete();
        }

        // 3. Once everything is saved, evaluate the measurements: the alerts and their emails are raised now.
        var alertsCreated = await externalCaregiverRegistrationService.GenerateAlertsAsync(
            healthRecordIds,
            cancellationToken);

        return new Result<CaregiverRegistrationResult, RegisterFamilyCaregiverError>.Success(
            new CaregiverRegistrationResult(user, olderAdultId, healthRecordIds, alertsCreated));
    }

    private static List<RegistrationFieldError> ValidateAccount(RegisterFamilyCaregiverCommand command)
    {
        var errors = new List<RegistrationFieldError>();

        try
        {
            _ = new Email(command.Email!);
        }
        catch (ArgumentException)
        {
            errors.Add(new RegistrationFieldError("email", RegistrationFieldCode.InvalidEmail));
        }

        if (!PasswordPolicy.IsValid(command.Password))
            errors.Add(new RegistrationFieldError("password", RegistrationFieldCode.InvalidPassword));

        CheckName(errors, "firstName", command.FirstName);
        CheckName(errors, "lastName", command.LastName);

        if (!command.AuthorizationAccepted)
            errors.Add(new RegistrationFieldError(
                "authorizationAccepted",
                RegistrationFieldCode.AuthorizationRequired));

        return errors;
    }

    private static void CheckName(List<RegistrationFieldError> errors, string field, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            errors.Add(new RegistrationFieldError(field, RegistrationFieldCode.Required));
        else if (value.Trim().Length > NameMaxLength)
            errors.Add(new RegistrationFieldError(field, RegistrationFieldCode.TooLong, NameMaxLength));
    }

    private static Result<CaregiverRegistrationResult, RegisterFamilyCaregiverError> Fail(
        RegisterFamilyCaregiverError error)
    {
        return new Result<CaregiverRegistrationResult, RegisterFamilyCaregiverError>.Failure(error);
    }
}
