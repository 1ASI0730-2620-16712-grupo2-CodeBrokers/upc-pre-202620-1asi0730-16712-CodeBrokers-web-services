# upc-pre-202620-1asi0730-16712-CodeBrokers-web-services
VitaLink RESTful Web Services developed with ASP.NET Core, Entity Framework Core and C# for the 1ASI0730 course.

## Requirements

- [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0)
- JetBrains Rider (or any IDE with .NET 10 support)

## Getting started

1. Clone the repository and switch to the `develop` branch.
2. Open `VitaLink.Platform.sln` in Rider.
3. Run the `http` launch profile, or from the repository root:

```bash
dotnet run --project VitaLink.Platform.API
```

The API listens on `http://localhost:5270`.

| Endpoint | Description |
|---|---|
| `GET /api/v1/health` | Health check |
| `/scalar` | Interactive API reference (Development only) |
| `/openapi/v1.json` | OpenAPI document (Development only) |

## Project structure

The solution is organized by bounded context, and each bounded context is split into layers:

```
VitaLink.Platform.API/
├── IAM/                    # Identity and Access Management
├── ElderCare/
├── PreventiveMonitoring/
├── Alerting/
├── CareCoordination/
├── Notification/
├── Marketing/
└── Shared/                 # Shared Kernel
    ├── Domain/             # Aggregates, entities, value objects, commands, queries, repository contracts
    ├── Application/        # Command and query services
    ├── Infrastructure/     # Persistence and external service adapters
    └── Interfaces/         # REST controllers, resources and assemblers
```

## Conventions

- **Routes:** every controller is exposed under the `/api/v1` prefix, and the `[controller]` token is converted to kebab-case (`HealthRecordsController` → `/api/v1/health-records`).
- **CORS:** allowed origins are defined per environment in `appsettings.{Environment}.json` under `Cors:AllowedOrigins`.
- **Branches:** GitFlow with `main`, `develop` and `feature/<description>` branches. Pull requests target `develop`.
- **Commits:** [Conventional Commits](https://www.conventionalcommits.org/) in English, using the bounded context as scope, e.g. `feat(alerting): add alert aggregate`.
