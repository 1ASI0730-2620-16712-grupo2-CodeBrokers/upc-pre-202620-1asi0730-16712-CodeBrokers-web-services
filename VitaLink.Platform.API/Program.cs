using Scalar.AspNetCore;
using VitaLink.Platform.API.Shared.Interfaces.ASP.Configuration;
using VitaLink.Platform.API.Notification.Domain.Services;
using VitaLink.Platform.API.Notification.Infrastructure.Email;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new KebabCaseRouteNamingConvention());
    options.UseGeneralRoutePrefix("api/v1");
});

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

var app = builder.Build();

// Configure the HTTP request pipeline.
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