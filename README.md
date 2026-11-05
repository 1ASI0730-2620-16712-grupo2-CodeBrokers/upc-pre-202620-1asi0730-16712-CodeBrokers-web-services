# upc-pre-202620-1asi0730-16712-CodeBrokers-web-services
VitaLink RESTful Web Services developed with ASP.NET Core, Entity Framework Core and C# for the 1ASI0730 course.

## Requirements

- [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0)
- JetBrains Rider (or any IDE with .NET 10 support)
- PostgreSQL 16 or later. The easiest way is [Docker Desktop](https://www.docker.com/products/docker-desktop/) with the provided `docker-compose.yml`.
- Entity Framework Core tools, to create migrations: `dotnet tool install --global dotnet-ef`

## Getting started

### 1. Start the database

**With Docker (recommended):** from the repository root, create your local `.env` file from the example and set your own password:

```bash
cp .env.example .env
```

Then start PostgreSQL:

```bash
docker compose up -d
```

This starts the `vitalink-postgres` container, listening only on `127.0.0.1:5432`, with a persistent volume. If port 5432 is already in use, change `POSTGRES_PORT` in your `.env`.

> `.env` contains your local password: it is ignored by git and must never be committed.
> `POSTGRES_PASSWORD` only applies the first time the volume is created. If you change it later, recreate the volume with `docker compose down -v` (this deletes the local data).

**Without Docker:** use any local PostgreSQL 16+ server and create a user that can create databases.

#### Inspect the database (optional)

With either option, you can connect to the running PostgreSQL server (for example, the `vitalink-postgres` container) with any client, such as TablePlus, pgAdmin, DBeaver or Rider's *Database* tool window. With Docker, use the values of your `.env`:

| Field | Value |
|---|---|
| Host | `localhost` |
| Port | `POSTGRES_PORT` (default `5432`) |
| User | `POSTGRES_USER` |
| Password | `POSTGRES_PASSWORD` |
| Database | `POSTGRES_DB` (default `vitalink`) |

After the API starts for the first time, the `__EFMigrationsHistory` table lists the applied migrations, and each bounded context creates its tables in its own schema.

### 2. Configure the connection string

The connection string is stored with [user secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets), outside the repository. Run once per machine, using the values of your `.env`:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=vitalink;Username=vitalink;Password=<your-password>" --project VitaLink.Platform.API
```

In deployed environments, the same value is provided with the `ConnectionStrings__DefaultConnection` environment variable.

### 3. Run the API

Open `VitaLink.Platform.sln` in Rider and run the `http` launch profile, or from the repository root:

```bash
dotnet run --project VitaLink.Platform.API
```

Pending migrations are applied automatically on startup. The API listens on `http://localhost:5270`.

| Endpoint | Description |
|---|---|
| `GET /api/v1/health` | Health check: `200 OK` when the database is reachable, `503 Service Unavailable` otherwise |
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

## Persistence

The persistence layer lives in `Shared` and is shared by every bounded context:

| Component | Location | Purpose |
|---|---|---|
| `AppDbContext` | `Shared/Infrastructure/Persistence/EntityFrameworkCore/Configuration` | Single EF Core context for all bounded contexts |
| `IBaseRepository<TEntity, TId>` / `BaseRepository<TEntity, TId>` | `Shared/Domain/Repositories` / `Shared/Infrastructure/.../Repositories` | Common CRUD operations. Repositories register changes but never save them |
| `IUnitOfWork` / `UnitOfWork` | Same folders | `CompleteAsync()` commits all pending changes of a use case at once |
| `IAuditableEntity` | `Shared/Domain/Model` | Entities implementing it get `CreatedAt` and `UpdatedAt` filled automatically |

Naming convention: tables are plural and every database identifier is snake_case (`HealthRecord.RecordedAt` → `health_records.recorded_at`). This is applied automatically.

### Mapping a new entity

1. **Identifiers:** use a strongly typed identifier on `Guid`, as in the class diagrams (e.g. `public sealed record AlertId(Guid Value)`). Primary keys are `UUID` columns.
2. **Configuration:** create an `IEntityTypeConfiguration<TEntity>` class in your bounded context, under `<BoundedContext>/Infrastructure/Persistence/EntityFrameworkCore/Configuration/`. It is discovered automatically: **do not edit `AppDbContext`**.
3. **Schema:** map the table to your bounded context schema, and convert the typed identifier:

```csharp
public class AlertConfiguration : IEntityTypeConfiguration<Alert>
{
    public void Configure(EntityTypeBuilder<Alert> builder)
    {
        builder.ToTable("alerts", "alerting");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasConversion(id => id.Value, value => new AlertId(value));
    }
}
```

| Bounded context | Schema |
|---|---|
| IAM | `iam` |
| ElderCare | `elder_care` |
| PreventiveMonitoring | `monitoring` |
| Alerting | `alerting` |
| CareCoordination | `care_coordination` |
| Notification | `notification` |
| Marketing | `marketing` |

4. **Repository:** define `IAlertRepository : IBaseRepository<Alert, AlertId>` in `Domain/Repositories` and implement it as `AlertRepository : BaseRepository<Alert, AlertId>, IAlertRepository`. Register it in `Program.cs`, inside your bounded context block.

### Migrations

Create a migration after changing the model, from the repository root:

```bash
dotnet ef migrations add <MigrationName> --project VitaLink.Platform.API --output-dir Shared/Infrastructure/Persistence/EntityFrameworkCore/Migrations
```

- Include **one migration per pull request**.
- Before creating it, merge the latest `develop` into your branch. If someone merged a migration before you, remove yours (`dotnet ef migrations remove --project VitaLink.Platform.API`) and create it again.

## Error handling and localization

### Returning errors

| Situation | How it is handled | HTTP response |
|---|---|---|
| Expected business failure (duplicate, invalid transition, not found) | The command service returns `Result<TValue, TError>` (`Shared/Application/Patterns`) and the controller translates the error | `4xx` with `ProblemDetails` |
| Invalid request (model binding or data annotations) | Automatic in `[ApiController]` controllers, or `ValidationProblem(ModelState)` | `400` with `ValidationProblemDetails` |
| Unexpected exception | `GlobalExceptionHandler` (`Shared/Interfaces/ASP/Middleware`) logs it | `500` with `ProblemDetails`, without exception details |

- Do not throw exceptions for expected business failures, and do not wrap controllers in a generic `try/catch`: unexpected exceptions already reach the global handler.
- Error responses follow RFC 7807 (`ProblemDetails`). Use `Problem(...)` or `ValidationProblem(...)` in controllers instead of returning plain strings.

Example of a controller translating a `Result`:

```csharp
return result switch
{
    Result<Alert, CreateAlertError>.Success success =>
        CreatedAtAction(nameof(GetAlert), new { id = success.Value.Id.Value },
            AlertResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

    Result<Alert, CreateAlertError>.Failure { Error: CreateAlertError.DuplicateAlert } =>
        Problem(statusCode: StatusCodes.Status409Conflict, title: localizer["AlertingAlertDuplicated"].Value),

    _ => Problem(statusCode: StatusCodes.Status500InternalServerError,
        title: localizer["UnexpectedServerError"].Value,
        detail: localizer["UnexpectedErrorProcessingRequest"].Value)
};
```

### Localization

The API answers in the language of the `Accept-Language` request header:

| `Accept-Language` | Culture used |
|---|---|
| none or unsupported | `en-US` (default) |
| `es-419` | `es-419` |
| any other Spanish variant (`es-PE`, `es`) | `es` |

The culture applied is returned in the `Content-Language` response header.

Messages live in `Resources/SharedResource.resx` (English) and `Resources/SharedResource.es.resx` (Spanish). To add a message:

1. Add the same key to **both** files. Prefix it with your bounded context, e.g. `AlertingAlertDuplicated` or `ElderCareOlderAdultNotFound`.
2. Inject `IStringLocalizer<SharedResource>` in the controller and read it with `localizer["Key"].Value`.
3. For data annotations, use the key as the error message: `[Required(ErrorMessage = "ElderCareFullNameRequired")]`.

Shared keys already available: `UnexpectedServerError`, `UnexpectedErrorProcessingRequest`, `InvalidModelStateTitle`, `ResourceNotFound` and `ConcurrencyConflict`.

To check a translation from the terminal:

```bash
curl -i -H "Accept-Language: es-419" http://localhost:5270/api/v1/alerts/not-a-guid
```

## Jira progress — Alerting

| Task | Implementation status | Scope |
| --- | --- | --- |
| T-8 — Alert aggregate and persistence | Implemented | Alert lifecycle and transition history, EF Core mappings, and the PostgreSQL `AddAlerts` migration. A unique index enforces one alert per health record. |
| T-10 — Alert queries | Implemented | Paginated alert and pending lists with status and patient filters, plus alert details by ID. |
| T-9 / US-14 — Automatic alert generation | Pending | Add `POST /api/v1/alerts` to evaluate thresholds and create a pending alert transactionally. Integration requires health-record lookup, active monitoring rules (T-18), and the RC-01 access policy. |
| T-30 / US-12 — Observation when attending an alert | Pending | Store an optional observation of up to 1,000 characters with the authenticated clinician and server timestamp, reject duplicates, and retain the draft in the web app after a save error. Depends on the attendance API (T-11) and web app integration. |

## Alert queries

- `GET /api/v1/alerts?status=PENDING&olderAdultId=<uuid>&page=1&pageSize=20`
- `GET /api/v1/alerts/pending?olderAdultId=<uuid>&page=1&pageSize=20`
- `GET /api/v1/alerts/<uuid>`

List filters are optional and combine with AND. Status accepts `PENDING`, `IN_REVIEW`,
`ATTENDED` and `CLOSED` (case insensitive). `olderAdultId` identifies the patient.
Pagination defaults to page 1 and 20 items, with a maximum page size of 100.
Results are ordered by `raisedAt` descending, then by ID for deterministic pagination.
Lists return `items`, `totalItems`, `page`, `pageSize` and `totalPages`; an empty or
out-of-range page returns HTTP 200 with an empty `items` array.
Invalid identifiers, status values or pagination return HTTP 400. A valid alert ID
that does not exist returns HTTP 404. Closed alerts remain queryable.

## Conventions

- **Routes:** every controller is exposed under the `/api/v1` prefix, and the `[controller]` token is converted to kebab-case (`HealthRecordsController` → `/api/v1/health-records`).
- **CORS:** allowed origins are defined per environment in `appsettings.{Environment}.json` under `Cors:AllowedOrigins`.
- **Secrets:** never commit passwords, keys or connection strings. Use user secrets locally and environment variables in deployed environments.
- **Branches:** GitFlow with `main`, `develop` and `feature/<description>` branches. Pull requests target `develop`.
- **Commits:** [Conventional Commits](https://www.conventionalcommits.org/) in English, using the bounded context as scope, e.g. `feat(alerting): add alert aggregate`.


## Continuous Deployment (CD)

VitaLink uses GitHub Actions, Docker Hub, and Microsoft Azure App Service to automate backend deployment.

### Deployment infrastructure

- **Cloud provider:** Microsoft Azure
- **Web App:** `vitalink-api-dev`
- **Container registry:** Docker Hub
- **Docker image:** `leorg15/vitalink-api:dev`
- **Database:** Azure Database for PostgreSQL
- **Container port:** `8080`
- **Deployment branch:** `feature/continuous-deployment`

### Deployment workflow

The workflow is defined in `.github/workflows/cd.yml`.

When changes are pushed to the `feature/continuous-deployment` branch:

1. GitHub Actions checks out the repository.
2. GitHub Actions authenticates with Docker Hub.
3. A new Docker image is built.
4. The image is pushed to Docker Hub with the `dev` tag and the commit SHA.
5. Docker Hub sends a webhook notification to Azure App Service.
6. Azure pulls the updated image and restarts the application.

### Deployment verification

The deployment can be verified through the health endpoint:

https://vitalink-api-dev-h4gdanfjc5ezbqep.canadacentral-01.azurewebsites.net/api/v1/health

A successful response returns:

```json
{
  "status": "Healthy",
  "database": "Up"
}
```

The response also includes a timestamp.

### Security

Docker Hub credentials are stored in GitHub Actions Secrets. Database credentials are configured through Azure App Service environment variables and are not included in the repository.
