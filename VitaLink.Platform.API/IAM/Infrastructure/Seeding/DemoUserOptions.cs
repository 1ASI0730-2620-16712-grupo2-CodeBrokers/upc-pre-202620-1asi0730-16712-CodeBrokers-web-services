namespace VitaLink.Platform.API.IAM.Infrastructure.Seeding;

/// <summary>
///     Configuration of the demo users, bound from the <c>IAM:DemoUsers</c> configuration section.
/// </summary>
/// <remarks>
///     <see cref="Password"/> is a secret: it must be set with <c>dotnet user-secrets</c>
///     or an environment variable, never in appsettings files or the repository.
/// </remarks>
public class DemoUserOptions
{
    /// <summary>Configuration section name.</summary>
    public const string SectionName = "IAM:DemoUsers";

    /// <summary>Indicates whether the demo users are created on startup.</summary>
    public bool Enabled { get; set; }

    /// <summary>Password shared by the demo users.</summary>
    public string? Password { get; set; }
}
