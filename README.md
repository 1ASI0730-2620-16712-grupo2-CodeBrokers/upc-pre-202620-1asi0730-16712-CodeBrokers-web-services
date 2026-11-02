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

## Conventions

- **Routes:** every controller is exposed under the `/api/v1` prefix, and the `[controller]` token is converted to kebab-case (`HealthRecordsController` → `/api/v1/health-records`).
- **CORS:** allowed origins are defined per environment in `appsettings.{Environment}.json` under `Cors:AllowedOrigins`.
- **Secrets:** never commit passwords, keys or connection strings. Use user secrets locally and environment variables in deployed environments.
- **Branches:** GitFlow with `main`, `develop` and `feature/<description>` branches. Pull requests target `develop`.
- **Commits:** [Conventional Commits](https://www.conventionalcommits.org/) in English, using the bounded context as scope, e.g. `feat(alerting): add alert aggregate`.
