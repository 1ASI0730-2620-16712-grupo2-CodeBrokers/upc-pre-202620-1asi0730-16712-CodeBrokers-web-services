using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;

namespace VitaLink.Platform.API.Shared.Interfaces.ASP.Configuration;

/// <summary>
///     MVC application-model convention that prepends a common route prefix
///     (e.g. <c>api/v1</c>) to every attribute-routed controller.
/// </summary>
/// <param name="routeTemplateProvider">The route template used as prefix.</param>
public class RoutePrefixConvention(IRouteTemplateProvider routeTemplateProvider) : IApplicationModelConvention
{
    private readonly AttributeRouteModel _routePrefix = new(routeTemplateProvider);

    /// <summary>
    ///     Applies the route prefix to all controller selectors.
    /// </summary>
    /// <param name="application">The application model to transform.</param>
    public void Apply(ApplicationModel application)
    {
        foreach (var selector in application.Controllers.SelectMany(c => c.Selectors))
            selector.AttributeRouteModel = selector.AttributeRouteModel != null
                ? AttributeRouteModel.CombineAttributeRouteModel(_routePrefix, selector.AttributeRouteModel)
                : _routePrefix;
    }
}

/// <summary>
///     Extension methods to register the <see cref="RoutePrefixConvention"/>.
/// </summary>
public static class RoutePrefixConventionExtensions
{
    /// <summary>
    ///     Adds a global route prefix to all controllers.
    /// </summary>
    /// <param name="options">The MVC options.</param>
    /// <param name="prefix">The route prefix, e.g. <c>api/v1</c>.</param>
    public static void UseGeneralRoutePrefix(this MvcOptions options, string prefix)
    {
        options.Conventions.Add(new RoutePrefixConvention(new RouteAttribute(prefix)));
    }
}
