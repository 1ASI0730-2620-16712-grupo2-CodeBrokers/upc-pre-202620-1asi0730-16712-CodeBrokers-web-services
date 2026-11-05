using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Scalar.AspNetCore;
using VitaLink.Platform.API.Alerting.Application.Internal.QueryServices;
using VitaLink.Platform.API.Alerting.Domain.Services;
using VitaLink.Platform.API.Alerting.Domain.Repositories;
using VitaLink.Platform.API.Alerting.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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

// Alerting Bounded Context
builder.Services.AddScoped<IAlertRepository, AlertRepository>();
builder.Services.AddScoped<IAlertQueryService, AlertQueryService>();

var app = builder.Build();

// Apply pending migrations on startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
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
