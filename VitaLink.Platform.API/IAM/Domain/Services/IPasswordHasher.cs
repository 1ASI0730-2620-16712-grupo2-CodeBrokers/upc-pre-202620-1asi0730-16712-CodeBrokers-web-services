using VitaLink.Platform.API.IAM.Domain.Model.ValueObjects;

namespace VitaLink.Platform.API.IAM.Domain.Services;

/// <summary>
///     Port for hashing and verifying user passwords.
/// </summary>
/// <remarks>
///     The domain depends only on this interface; the algorithm (BCrypt) lives in the infrastructure layer.
/// </remarks>
public interface IPasswordHasher
{
    /// <summary>
    ///     Hashes a plain-text password.
    /// </summary>
    /// <param name="plainText">Password typed by the user.</param>
    /// <returns>The password hash to store.</returns>
    PasswordHash Hash(string plainText);

    /// <summary>
    ///     Checks whether a plain-text password matches a stored hash.
    /// </summary>
    /// <param name="plainText">Password typed by the user.</param>
    /// <param name="hash">Stored password hash.</param>
    /// <returns><c>true</c> when the password matches; otherwise <c>false</c>.</returns>
    bool Matches(string plainText, PasswordHash hash);
}
