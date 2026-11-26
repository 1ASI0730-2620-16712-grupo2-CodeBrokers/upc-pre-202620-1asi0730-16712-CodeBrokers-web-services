using VitaLink.Platform.API.IAM.Domain.Model.Aggregates;
using VitaLink.Platform.API.IAM.Interfaces.REST.Resources;

namespace VitaLink.Platform.API.IAM.Interfaces.REST.Transform;

/// <summary>
///     Builds the public representation of a user.
/// </summary>
public static class UserResourceFromEntityAssembler
{
    /// <param name="entity">User aggregate.</param>
    public static UserResource ToResourceFromEntity(User entity) =>
        new(entity.Id.Value, entity.Email.Value, UserRoleText.ToText(entity.Role), entity.IsVerified);
}
