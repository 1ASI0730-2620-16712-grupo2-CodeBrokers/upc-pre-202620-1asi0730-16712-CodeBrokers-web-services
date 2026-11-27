using VitaLink.Platform.API.IAM.Domain.Model.Commands;
using VitaLink.Platform.API.IAM.Domain.Model.ValueObjects;
using VitaLink.Platform.API.IAM.Interfaces.REST.Resources;

namespace VitaLink.Platform.API.IAM.Interfaces.REST.Transform;

/// <summary>
///     Builds a <see cref="SignUpCommand"/> from the request body.
/// </summary>
public static class SignUpCommandFromResourceAssembler
{
    /// <param name="resource">Request body.</param>
    /// <param name="role">Role already parsed from <see cref="SignUpResource.Role"/>.</param>
    public static SignUpCommand ToCommandFromResource(SignUpResource resource, UserRole role) =>
        new(
            resource.Email,
            resource.Password,
            role,
            resource.FirstName,
            resource.LastName,
            resource.Specialty,
            resource.LicenseNumber,
            resource.ClinicId);
}
