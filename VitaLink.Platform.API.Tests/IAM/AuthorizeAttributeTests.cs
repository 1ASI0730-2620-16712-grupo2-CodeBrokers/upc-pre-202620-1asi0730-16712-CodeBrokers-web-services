using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using VitaLink.Platform.API.IAM.Domain.Model.Aggregates;
using VitaLink.Platform.API.IAM.Domain.Model.ValueObjects;
using VitaLink.Platform.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using VitaLink.Platform.API.IAM.Infrastructure.Pipeline.Middleware.Components;

namespace VitaLink.Platform.API.Tests.IAM;

/// <summary>
///     Authorization matrix by role (TS-04 CA-03): allowed access, another role and no session.
/// </summary>
public class AuthorizeAttributeTests
{
    private static AuthorizationFilterContext Context(User? user, params object[] metadata)
    {
        var http = new DefaultHttpContext();
        if (user is not null) http.Items[RequestAuthorizationMiddleware.UserKey] = user;
        var action = new ActionContext(http, new RouteData(), new ActionDescriptor { EndpointMetadata = [.. metadata] });
        return new AuthorizationFilterContext(action, []);
    }

    private static User UserWith(UserRole role) => new(new Email("user@vitalink.test"), new PasswordHash("hash"), role);

    private static int? Status(AuthorizationFilterContext context) => (context.Result as ObjectResult)?.StatusCode;

    [Fact]
    public void NoSessionIsUnauthorized()
    {
        var context = Context(null);
        new AuthorizeAttribute().OnAuthorization(context);
        Assert.Equal(StatusCodes.Status401Unauthorized, Status(context));
    }

    [Theory]
    [InlineData(UserRole.CareProvider)]
    [InlineData(UserRole.FamilyCaregiver)]
    [InlineData(UserRole.OlderAdult)]
    public void AnySignedInRoleIsAllowedWithoutRoles(UserRole role)
    {
        var context = Context(UserWith(role));
        new AuthorizeAttribute().OnAuthorization(context);
        Assert.Null(context.Result);
    }

    [Theory]
    [InlineData(UserRole.CareProvider, null)]
    [InlineData(UserRole.FamilyCaregiver, StatusCodes.Status403Forbidden)]
    [InlineData(UserRole.OlderAdult, StatusCodes.Status403Forbidden)]
    public void CareProviderOnlyAction(UserRole role, int? expected)
    {
        var context = Context(UserWith(role));
        new AuthorizeAttribute(UserRole.CareProvider).OnAuthorization(context);
        Assert.Equal(expected, Status(context));
    }

    [Theory]
    [InlineData(UserRole.CareProvider, null)]
    [InlineData(UserRole.FamilyCaregiver, null)]
    [InlineData(UserRole.OlderAdult, StatusCodes.Status403Forbidden)]
    public void CareProviderOrFamilyAction(UserRole role, int? expected)
    {
        var context = Context(UserWith(role));
        new AuthorizeAttribute(UserRole.CareProvider, UserRole.FamilyCaregiver).OnAuthorization(context);
        Assert.Equal(expected, Status(context));
    }

    [Fact]
    public void AllowAnonymousSkipsTheCheck()
    {
        var context = Context(null, new AllowAnonymousAttribute());
        new AuthorizeAttribute(UserRole.CareProvider).OnAuthorization(context);
        Assert.Null(context.Result);
    }
}
