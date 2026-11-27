using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace VitaLink.Platform.API.Shared.Infrastructure.Interfaces.AspNetCore.OpenApi;

/// <summary>
///     OpenAPI document transformer that documents the API and its JWT Bearer security scheme.
/// </summary>
/// <remarks>
///     The token is returned by <c>POST /api/v1/authentication/sign-in</c>. Scalar shows an "Authorize" field where it
///     can be pasted; the endpoints that require it are enforced by the authorization of the IAM bounded context.
/// </remarks>
public sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    private const string SchemeName = "Bearer";

    /// <summary>
    ///     Adds the API description and the Bearer security scheme to the document.
    /// </summary>
    /// <param name="document">The OpenAPI document to transform.</param>
    /// <param name="context">The document transformer context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Info ??= new OpenApiInfo();
        document.Info.Title = "VitaLink Platform API";
        document.Info.Description =
            "RESTful API of VitaLink: preventive monitoring of older adults (health records, monitoring rules, " +
            "alerts and notifications). Send the Accept-Language header (en-US or es) to choose the language " +
            "of the messages. The examples use synthetic data.";

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes[SchemeName] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            In = ParameterLocation.Header,
            BearerFormat = "JWT",
            Description = "JWT returned by POST /api/v1/authentication/sign-in."
        };

        document.Security ??= [];
        document.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(SchemeName, document)] = []
        });

        return Task.CompletedTask;
    }
}
