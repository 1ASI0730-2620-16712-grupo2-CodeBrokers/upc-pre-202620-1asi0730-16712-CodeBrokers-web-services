using VitaLink.Platform.API.IAM.Domain.Model.ValueObjects;

namespace VitaLink.Platform.API.IAM.Domain.Model.Aggregates;

/// <summary>
///     User aggregate root: the account a person uses to sign in to VitaLink under a role.
/// </summary>
/// <remarks>
///     Accounts are never deleted: they are deactivated, so the records they authored
///     (health records, alert status changes, care records) keep a valid author.
/// </remarks>
public class User
{
    /// <summary>Required by Entity Framework Core.</summary>
    protected User()
    {
        Id = null!;
        Email = null!;
        PasswordHash = null!;
    }

    /// <summary>
    ///     Creates an active, unverified user account.
    /// </summary>
    /// <param name="email">Email used to sign in.</param>
    /// <param name="passwordHash">Hash of the user's password.</param>
    /// <param name="role">Role the user operates under.</param>
    public User(Email email, PasswordHash passwordHash, UserRole role)
    {
        ArgumentNullException.ThrowIfNull(email);
        ArgumentNullException.ThrowIfNull(passwordHash);
        if (!Enum.IsDefined(role))
            throw new ArgumentOutOfRangeException(nameof(role), role, "User role is not valid.");

        Id = UserId.New();
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        IsVerified = false;
        IsActive = true;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>User identifier.</summary>
    public UserId Id { get; private set; }

    /// <summary>Email used to sign in (unique).</summary>
    public Email Email { get; private set; }

    /// <summary>Hash of the user's password.</summary>
    public PasswordHash PasswordHash { get; private set; }

    /// <summary>Role the user operates under.</summary>
    public UserRole Role { get; private set; }

    /// <summary>Indicates whether the account was verified.</summary>
    public bool IsVerified { get; private set; }

    /// <summary>Indicates whether the account is active (not deactivated).</summary>
    public bool IsActive { get; private set; }

    /// <summary>Creation moment.</summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>Moment of the last successful sign-in, if any.</summary>
    public DateTimeOffset? LastAccessAt { get; private set; }

    /// <summary>
    ///     Marks the account as verified.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the account is deactivated.</exception>
    public void VerifyAccount()
    {
        EnsureNotDeactivated();
        IsVerified = true;
    }

    /// <summary>
    ///     Records a successful sign-in.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the account is deactivated.</exception>
    public void RegisterAccess()
    {
        EnsureNotDeactivated();
        LastAccessAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    ///     Replaces the password hash.
    /// </summary>
    /// <param name="newHash">Hash of the new password.</param>
    /// <exception cref="InvalidOperationException">Thrown when the account is deactivated.</exception>
    public void ChangePassword(PasswordHash newHash)
    {
        ArgumentNullException.ThrowIfNull(newHash);
        EnsureNotDeactivated();
        PasswordHash = newHash;
    }

    /// <summary>
    ///     Deactivates the account. A deactivated account cannot sign in.
    /// </summary>
    public void Deactivate() => IsActive = false;

    /// <summary>
    ///     Indicates whether the user can sign in: the account must be active and verified.
    /// </summary>
    public bool IsEnabled() => IsActive && IsVerified;

    private void EnsureNotDeactivated()
    {
        if (!IsActive)
            throw new InvalidOperationException("User account is deactivated.");
    }
}
