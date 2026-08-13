# Hospital SaaS Backend

.NET 10 modular-monolith backend using Clean Architecture, EF Core with SQL Server, JWT authentication, permission authorization, and centralized tenant isolation.

## Architecture

- `Domain`: framework-independent entities and business rules.
- `Application`: CQRS use cases, validation, DTOs, result models, and infrastructure ports.
- `Infrastructure`: EF Core persistence, SQL Server, authentication, storage, caching, and tenant context.
- `Contracts`: stable HTTP request and response models.
- `Api`: thin controllers, middleware, authorization, OpenAPI, and health endpoints.

Dependency direction is `Domain <- Application <- Infrastructure`, with API composing Application, Infrastructure, and Contracts.

Every business endpoint uses an explicit command or query through an in-process request dispatcher. The dispatcher composes FluentValidation, structured logging, performance monitoring, and transaction behaviors. Transactions apply only to commands; queries remain read-only.

## Run locally

Requirements: .NET 10 SDK and SQL Server at `DESKTOP-O8LF2FG`. The development connection string is in `src/HospitalSaaS.Api/appsettings.json`. Override secrets with environment variables or user secrets; never commit production secrets.

```powershell
dotnet restore HospitalSaaS.sln
dotnet tool restore
dotnet build HospitalSaaS.sln
dotnet ef database update --project src/HospitalSaaS.Infrastructure --startup-project src/HospitalSaaS.Api
dotnet run --project src/HospitalSaaS.Api
```

Create migrations with:

```powershell
dotnet ef migrations add MigrationName --project src/HospitalSaaS.Infrastructure --startup-project src/HospitalSaaS.Api --output-dir Persistence/Migrations
```

Anonymous authentication calls may pass `X-Tenant-ID`. Authenticated requests derive tenant identity exclusively from the signed JWT. Tenant-owned EF entities have global query filters. SaaS-wide filter bypass is scoped, disposable, and restricted to explicitly authorized super administrators.

## Initial administrator

No password is stored in source. Supply these settings before first startup if an initial hospital administrator is required:

```powershell
dotnet user-secrets set "InitialAdmin:Email" "admin@example.com" --project src/HospitalSaaS.Api
dotnet user-secrets set "InitialAdmin:Password" "a-long-random-development-password" --project src/HospitalSaaS.Api
dotnet user-secrets set "InitialAdmin:TenantName" "Example Hospital" --project src/HospitalSaaS.Api
dotnet user-secrets set "InitialAdmin:TenantSlug" "example-hospital" --project src/HospitalSaaS.Api
```

The idempotent startup seeder creates the permission catalog and configured administrator only when all required settings are present. Changing a password revokes every active refresh token for that user.

## Implemented API areas

- Tenants, users, roles, permissions, login, refresh, logout, revoke, current user, and password changes
- Patients, departments, doctors, complete appointment workflow, wards, rooms, beds, admissions, and transfer history
- Medical records with diagnoses, prescriptions, pharmacy stock ledger, laboratory test catalog/results history, and radiology catalog/results history
- Invoices, payments, refunds, insurance, inventory ledger, suppliers, purchase orders, and stock receipt posting
- Notifications, document storage, correlation-aware old/new-value audit logs, subscriptions, staff, and dashboard summaries

All routes use the `/api/v1` prefix. OpenAPI is available in Development and health status at `/health`. SMTP email and HTTP SMS delivery are provider-configurable through the `Email` and `Sms` configuration sections; credentials should be supplied by environment variables or user secrets. Background work runs through a bounded hosted queue with graceful cancellation.

## Verification

```powershell
dotnet csharpier check src tests
dotnet build HospitalSaaS.sln --no-restore
dotnet test HospitalSaaS.sln --no-build --no-restore
dotnet ef migrations has-pending-model-changes --project src/HospitalSaaS.Infrastructure --startup-project src/HospitalSaaS.Api --no-build
dotnet list HospitalSaaS.sln package --vulnerable --include-transitive
```

The solution currently provides 76 passing domain, application, integration, and API tests. The SQL integration suite uses rolled-back transactions to verify mappings, constraints, and cross-tenant isolation. API tests cover health, safe correlation IDs, tenant requirements, permissions, authentication boundaries, and validation responses.
