namespace VitaLink.Platform.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;

/// <summary>
///     Marks an action that does not require a signed-in user, even if its controller has <see cref="AuthorizeAttribute" />.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class AllowAnonymousAttribute : Attribute;
