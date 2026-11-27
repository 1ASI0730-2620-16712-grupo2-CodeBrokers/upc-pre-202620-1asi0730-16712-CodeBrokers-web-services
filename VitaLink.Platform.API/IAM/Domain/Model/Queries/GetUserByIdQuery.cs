using VitaLink.Platform.API.IAM.Domain.Model.ValueObjects;

namespace VitaLink.Platform.API.IAM.Domain.Model.Queries;

/// <summary>
///     Query to read one user by identifier.
/// </summary>
/// <param name="UserId">The user identifier.</param>
public sealed record GetUserByIdQuery(UserId UserId);
