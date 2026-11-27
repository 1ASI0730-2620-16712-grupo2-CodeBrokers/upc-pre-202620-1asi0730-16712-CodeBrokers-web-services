using System.Text.Json.Nodes;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace VitaLink.Platform.API.Shared.Infrastructure.Interfaces.AspNetCore.OpenApi;

/// <summary>
///     OpenAPI schema transformer that adds request and response examples to the main resources.
/// </summary>
/// <remarks>
///     The examples are matched by resource name and use synthetic data only (no real personal data).
/// </remarks>
public sealed class ExamplesSchemaTransformer : IOpenApiSchemaTransformer
{
    private static readonly Dictionary<string, string> Examples = new()
    {
        ["SignInResource"] = """
            { "email": "dr.demo@vitalink.test", "password": "Demo1234!" }
            """,
        ["SignUpResource"] = """
            { "email": "familiar.demo@vitalink.test", "password": "Demo1234!", "role": "FAMILY_CAREGIVER",
              "firstName": "Ana", "lastName": "Demo" }
            """,
        ["CreateHealthRecordResource"] = """
            { "recordTypeCode": "HEART_RATE", "measurementValue": 72, "unit": "bpm", "note": null,
              "source": "CARE_PROVIDER", "recordedBy": "00000000-0000-4000-8000-0000000000d1" }
            """,
        ["HealthRecordResource"] = """
            { "id": "6400f19a-9f7c-4349-9c62-095094a715d1", "olderAdultId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
              "recordTypeCode": "HEART_RATE", "measurementValue": 72, "note": null, "source": "CARE_PROVIDER",
              "recordedBy": "00000000-0000-4000-8000-0000000000d1", "recordedAt": "2026-11-26T15:00:00+00:00" }
            """,
        ["CreateMonitoringRuleResource"] = """
            { "olderAdultId": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "recordTypeCode": "HEART_RATE",
              "minThreshold": 50, "maxThreshold": 110, "severity": "HIGH" }
            """,
        ["CreateAlertResource"] = """
            { "healthRecordId": "6400f19a-9f7c-4349-9c62-095094a715d1" }
            """,
        ["AlertResource"] = """
            { "id": "c2f287c1-fe96-4ef8-9f12-58fac0d3408d", "olderAdultId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
              "healthRecordId": "6400f19a-9f7c-4349-9c62-095094a715d1",
              "monitoringRuleId": "c501c409-39ab-4e8b-a4e8-da50d536816b", "assignedProviderId": null,
              "severity": "HIGH", "status": "PENDING", "raisedAt": "2026-11-26T15:00:01+00:00",
              "lastStatusChangeAt": "2026-11-26T15:00:01+00:00", "version": 1 }
            """,
        ["UpdateAlertStatusResource"] = """
            { "status": "IN_REVIEW", "changedBy": "00000000-0000-4000-8000-0000000000d1", "version": 1 }
            """,
        ["CreateAlertObservationResource"] = """
            { "text": "Se llamó al familiar; la medición se repetirá en 30 minutos.",
              "createdBy": "00000000-0000-4000-8000-0000000000d1" }
            """
    };

    /// <summary>
    ///     Sets the example of the schema when the resource has one.
    /// </summary>
    /// <param name="schema">The schema being generated.</param>
    /// <param name="context">The schema transformer context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context,
        CancellationToken cancellationToken)
    {
        if (Examples.TryGetValue(context.JsonTypeInfo.Type.Name, out var json))
            schema.Examples = [JsonNode.Parse(json)!];

        return Task.CompletedTask;
    }
}
