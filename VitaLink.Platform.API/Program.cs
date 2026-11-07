using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Scalar.AspNetCore;
using VitaLink.Platform.API.Alerting.Application.Internal.QueryServices;
using VitaLink.Platform.API.Alerting.Application.Internal.CommandServices;
using VitaLink.Platform.API.Alerting.Application.Internal.OutboundServices.ACL;
using VitaLink.Platform.API.Alerting.Domain.Services;
using VitaLink.Platform.API.Alerting.Domain.Repositories;
using VitaLink.Platform.API.Alerting.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using VitaLink.Platform.API.IAM.Domain.Repositories;
using VitaLink.Platform.API.IAM.Domain.Services;
using VitaLink.Platform.API.IAM.Infrastructure.Hashing.BCrypt;
using VitaLink.Platform.API.IAM.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using VitaLink.Platform.API.IAM.Infrastructure.Seeding;
using VitaLink.Platform.API.Notification.Domain.Repositories;
using VitaLink.Platform.API.Notification.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using VitaLink.Platform.API.Resources;
using VitaLink.Platform.API.Shared.Domain.Repositories;
using VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using VitaLink.Platform.API.Shared.Interfaces.ASP.Configuration;
using VitaLink.Platform.API.Shared.Interfaces.ASP.Middleware;
using VitaLink.Platform.API.Notification.Domain.Services;
using VitaLink.Platform.API.Notification.Infrastructure.Email;
// ElderCare
using VitaLink.Platform.API.ElderCare.Domain.Repositories;
using VitaLink.Platform.API.ElderCare.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using VitaLink.Platform.API.ElderCare.Application.Services;
using VitaLink.Platform.API.ElderCare.Application.Internal.CommandServices;

// Notification
using VitaLink.Platform.API.Notification.Application.Internal.CommandServices;
using VitaLink.Platform.API.Notification.Application.Services;
using VitaLink.Platform.API.Notification.Application.Internal.QueryServices;

// PreventiveMonitoring
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Repositories;
using VitaLink.Platform.API.PreventiveMonitoring.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Services;
using VitaLink.Platform.API.PreventiveMonitoring.Application.Internal.CommandServices;
using VitaLink.Platform.API.PreventiveMonitoring.Application.Internal.QueryServices;
using VitaLink.Platform.API.PreventiveMonitoring.Application.ACL;
using VitaLink.Platform.API.PreventiveMonitoring.Interfaces.ACL;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// En el bloque de Preventive Monitoring Services:
builder.Services.AddScoped<IRecordTypeRepository, RecordTypeRepository>();
builder.Services.AddScoped<IHealthRecordCommandService, HealthRecordCommandService>();
builder.Services.AddScoped<IHealthRecordQueryService, HealthRecordQueryService>();

builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new KebabCaseRouteNamingConvention());
    options.UseGeneralRoutePrefix("api/v1");
})
    // Data annotation messages (e.g. [Required(ErrorMessage = "Key")]) are looked up in SharedResource
    .AddDataAnnotationsLocalization(options =>
        options.DataAnnotationLocalizerProvider = (_, factory) => factory.Create(typeof(SharedResource)));

// Error handling: unexpected exceptions are returned as RFC 7807 ProblemDetails
builder.Services.AddProblemDetails(options =>
    options.CustomizeProblemDetails = context =>
    {
        // Validation errors (automatic 400 and ValidationProblem()) get a localized title
        if (context.ProblemDetails is not HttpValidationProblemDetails) return;
        var localizer = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<SharedResource>>();
        context.ProblemDetails.Title = localizer["InvalidModelStateTitle"].Value;
    });

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Localization: messages come from Resources/SharedResource.*.resx
builder.Services.AddLocalization();

// CORS: allowed origins are defined per environment in appsettings.{Environment}.json
const string allowedOriginsPolicy = "AllowedOriginsPolicy";
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy(allowedOriginsPolicy, policy =>
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// Database: PostgreSQL connection string from user-secrets (Development) or environment variables
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException(
        "Connection string 'DefaultConnection' is not set. Configure it with dotnet user-secrets (see README).");

builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    options.UseNpgsql(connectionString)
        .UseLoggerFactory(serviceProvider.GetRequiredService<ILoggerFactory>())
        .EnableDetailedErrors();

    if (builder.Environment.IsDevelopment())
        options.EnableSensitiveDataLogging();
});

// Shared Bounded Context Injection Configuration
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// IAM Bounded Context
builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.Configure<DemoUserOptions>(builder.Configuration.GetSection(DemoUserOptions.SectionName));
builder.Services.AddScoped<DemoUserSeeder>();

// Notification Bounded Context
builder.Services.Configure<EmailSenderOptions>(
    builder.Configuration.GetSection(EmailSenderOptions.SectionName));

builder.Services.AddHttpClient(ResendEmailSenderService.HttpClientName, client =>
    client.BaseAddress = new Uri("https://api.resend.com/"));

var emailOptions = builder.Configuration.GetSection(EmailSenderOptions.SectionName).Get<EmailSenderOptions>()
                   ?? new EmailSenderOptions();

if (emailOptions.HasApiKey)
    builder.Services.AddScoped<IEmailSenderService, ResendEmailSenderService>();
else
    builder.Services.AddScoped<IEmailSenderService, LoggingEmailSenderService>();

builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationCommandService, NotificationCommandService>();
builder.Services.AddScoped<INotificationQueryService, NotificationQueryService>();

// Alerting Bounded Context
builder.Services.AddScoped<IAlertRepository, AlertRepository>();
builder.Services.AddScoped<IAlertQueryService, AlertQueryService>();
builder.Services.AddScoped<IAlertStatusCommandService, AlertStatusCommandService>();
builder.Services.AddScoped<IAlertCommandService, AlertCommandService>();
builder.Services.AddScoped<IAlertObservationRepository, AlertObservationRepository>();
builder.Services.AddScoped<IAlertObservationCommandService, AlertObservationCommandService>();
builder.Services.AddScoped<IAlertObservationQueryService, AlertObservationQueryService>();
builder.Services.AddScoped<ExternalMonitoringService>();

// Preventive Monitoring Bounded Context
builder.Services.AddScoped<IHealthRecordRepository, HealthRecordRepository>();
builder.Services.AddScoped<IMonitoringRuleRepository, MonitoringRuleRepository>();
builder.Services.AddScoped<RiskEvaluationService>();
builder.Services.AddScoped<IMonitoringRuleCommandService, MonitoringRuleCommandService>();
builder.Services.AddScoped<IMonitoringRuleQueryService, MonitoringRuleQueryService>();
builder.Services.AddScoped<IMonitoringContextFacade, MonitoringContextFacade>();

// Elder Care Bounded Context
builder.Services.AddScoped<ICareProviderRepository, CareProviderRepository>();
builder.Services.AddScoped<IOlderAdultRepository, OlderAdultRepository>();
builder.Services.AddScoped<IOlderAdultCommandService, OlderAdultCommandService>();

var app = builder.Build();

// Apply pending migrations and create the demo users (when enabled) on startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();

    await scope.ServiceProvider.GetRequiredService<DemoUserSeeder>().SeedAsync();
}

// Configure the HTTP request pipeline.

// Localization runs first so error responses also use the request culture (Accept-Language).
// en-US is the default; es-419 covers Latin American Spanish and es any other Spanish variant.
string[] supportedCultures = ["en-US", "es-419", "es"];

var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

localizationOptions.ApplyCurrentCultureToResponseHeaders = true;
app.UseRequestLocalization(localizationOptions);

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseCors(allowedOriginsPolicy);

app.UseAuthorization();

app.MapControllers();

app.Run();
