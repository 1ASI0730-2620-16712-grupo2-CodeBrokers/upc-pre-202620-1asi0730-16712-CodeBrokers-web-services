using VitaLink.Platform.API.IAM.Domain.Model.Commands;
using VitaLink.Platform.API.IAM.Interfaces.REST.Resources;

namespace VitaLink.Platform.API.IAM.Interfaces.REST.Transform;

/// <summary>
///     Builds a <see cref="SignInCommand"/> from the request body.
/// </summary>
public static class SignInCommandFromResourceAssembler
{
    /// <param name="resource">Request body.</param>
    public static SignInCommand ToCommandFromResource(SignInResource resource) =>
        new(resource.Email, resource.Password);
}
