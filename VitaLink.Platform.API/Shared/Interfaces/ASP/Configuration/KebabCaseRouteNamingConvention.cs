using Microsoft.AspNetCore.Mvc.ApplicationModels;
using VitaLink.Platform.API.Shared.Interfaces.ASP.Configuration.Extensions;

namespace VitaLink.Platform.API.Shared.Interfaces.ASP.Configuration;

/// <summary>
///     MVC controller-model convention that rewrites <c>[controller]</c> route tokens
///     to their kebab-case equivalents (e.g. <c>HealthRecords</c> → <c>health-records</c>).
/// </summary>
public class KebabCaseRouteNamingConvention : IControllerModelConvention
{
    /// <summary>
    ///     Applies the kebab-case naming transformation to all selectors on the controller and its actions.
    /// </summary>
    /// <param name="controller">The controller model to transform.</param>
    public void Apply(ControllerModel controller)
    {
        foreach (var selector in controller.Selectors)
            selector.AttributeRouteModel = ReplaceControllerTemplate(selector, controller.ControllerName);

        foreach (var selector in controller.Actions.SelectMany(a => a.Selectors))
            selector.AttributeRouteModel = ReplaceControllerTemplate(selector, controller.ControllerName);
    }

    private static AttributeRouteModel? ReplaceControllerTemplate(SelectorModel selector, string name)
    {
        return selector.AttributeRouteModel != null
            ? new AttributeRouteModel
            {
                Template = selector.AttributeRouteModel.Template?.Replace("[controller]", name.ToKebabCase())
            }
            : null;
    }
}
