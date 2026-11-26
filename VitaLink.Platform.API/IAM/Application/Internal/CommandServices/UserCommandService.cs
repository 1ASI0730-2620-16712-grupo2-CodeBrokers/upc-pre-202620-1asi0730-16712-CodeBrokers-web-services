using VitaLink.Platform.API.IAM.Application.CommandServices;
using VitaLink.Platform.API.IAM.Application.Internal.OutboundServices;
using VitaLink.Platform.API.IAM.Application.Results;
using VitaLink.Platform.API.IAM.Domain.Model.Aggregates;
using VitaLink.Platform.API.IAM.Domain.Model.Commands;
using VitaLink.Platform.API.IAM.Domain.Model.Errors;
using VitaLink.Platform.API.IAM.Domain.Model.ValueObjects;
using VitaLink.Platform.API.IAM.Domain.Repositories;
using VitaLink.Platform.API.IAM.Domain.Services;
using VitaLink.Platform.API.Shared.Application.Model;
using VitaLink.Platform.API.Shared.Domain.Repositories;

namespace VitaLink.Platform.API.IAM.Application.Internal.CommandServices;

/// <inheritdoc />
public class UserCommandService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IUnitOfWork unitOfWork) : IUserCommandService
{
    /// <inheritdoc />
    public async Task<Result<User, SignUpError>> Handle(
        SignUpCommand command,
        CancellationToken cancellationToken = default)
    {
        // The older adult account is created by the family caregiver, never by sign-up.
        if (command.Role is not (UserRole.CareProvider or UserRole.FamilyCaregiver))
            return new Result<User, SignUpError>.Failure(SignUpError.RoleNotAllowed);

        Email email;
        try
        {
            email = new Email(command.Email);
        }
        catch (ArgumentException)
        {
            return new Result<User, SignUpError>.Failure(SignUpError.InvalidEmail);
        }

        if (!PasswordPolicy.IsValid(command.Password))
            return new Result<User, SignUpError>.Failure(SignUpError.InvalidPassword);

        if (await userRepository.ExistsByEmailAsync(email, cancellationToken))
            return new Result<User, SignUpError>.Failure(SignUpError.EmailAlreadyRegistered);

        var user = new User(email, passwordHasher.Hash(command.Password), command.Role);

        // Email verification is out of scope: accounts are created ready to sign in.
        user.VerifyAccount();

        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);

        return new Result<User, SignUpError>.Success(user);
    }

    /// <inheritdoc />
    public async Task<Result<AuthenticatedUser, SignInError>> Handle(
        SignInCommand command,
        CancellationToken cancellationToken = default)
    {
        Email email;
        try
        {
            email = new Email(command.Email);
        }
        catch (ArgumentException)
        {
            return new Result<AuthenticatedUser, SignInError>.Failure(SignInError.InvalidCredentials);
        }

        var user = await userRepository.FindByEmailAsync(email, cancellationToken);

        // Same error for an unknown email and a wrong password, so the API does not reveal which emails exist.
        if (user is null || !passwordHasher.Matches(command.Password, user.PasswordHash))
            return new Result<AuthenticatedUser, SignInError>.Failure(SignInError.InvalidCredentials);

        if (!user.IsEnabled())
            return new Result<AuthenticatedUser, SignInError>.Failure(SignInError.AccountDisabled);

        user.RegisterAccess();
        userRepository.Update(user);
        await unitOfWork.CompleteAsync(cancellationToken);

        return new Result<AuthenticatedUser, SignInError>.Success(
            new AuthenticatedUser(user, tokenService.Generate(user)));
    }
}
