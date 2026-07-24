# Unmatched

## Overview

.NET 9 microservices backend + Angular 19 (primary UI) and Blazor Server (secondary UI).

- `Services/{Catalog,Match,Player,Statistics}` — one microservice per bounded context, each split into
  `*.Api` (controllers, composition root) / `*.Contracts` (DTOs shared across process boundaries) /
  `*.Domain` (entities, domain logic) / `*.EntityFramework` (persistence) / `*.Tests` (xUnit).
- `Unmatched.Api` — API gateway.
- `Unmatched` — shared client library (HttpClients, Dtos, Mapping, Extensions) consumed by the UIs.
- `Unmatched.Initializer` — startup/seed/migration jobs.
- `Shared/Unmatched.Common.EntityFramework` — cross-service EF Core building blocks.
- `Unmatched.UI.Angular` — primary frontend (Angular 19, Angular Material, Karma/Jasmine).
- `Unmatched.UI.BlazorServer` — secondary frontend.
- Kafka (via `docker-compose.yml`, services `kafka`/`zookeeper`/`kafka-ui`) for async/event communication between services.

Run everything with `docker-compose.yml` or `build_and_run.bat`.

## Docker / local SQL Server setup

`docker-compose.yml` does **not** containerize SQL Server — every service connects to
`host.docker.internal,1433` using `DB_USER`/`DB_PASS` from a local `.env` (see `.env.example`). This
means a real SQL Server instance must already be running on the host with:

- **Mixed Mode authentication enabled** (SQL Server won't accept the SQL login otherwise) — enabling it
  requires restarting the `MSSQLSERVER` service to take effect.
- The `unmatched_user` login created with `CHECK_POLICY = OFF, CHECK_EXPIRATION = OFF` — otherwise SQL
  Server treats the password as expired (error 18488) after a while and every service crashes on startup.
- The `Unmatched` database **restored manually** from one of the `.bacpac` files in `db/` (pick the
  newest). Services call `EnsureCreated()` + `Migrate()` on startup but do **not** auto-restore data, and
  `unmatched_user` normally lacks server-level `CREATE DATABASE` rights — if the database doesn't exist,
  every service crashes with error 262 (`CREATE DATABASE permission denied`) instead of just creating an
  empty one.
- `unmatched_user` mapped to the `Unmatched` database with `db_owner`:
  ```sql
  USE Unmatched;
  CREATE USER [unmatched_user] FOR LOGIN [unmatched_user];
  ALTER ROLE db_owner ADD MEMBER [unmatched_user];
  ```

`Unmatched.UI.Angular` and `Unmatched.Initializer` are **not** part of `docker-compose.yml` — run them
separately (`npm start` / `dotnet run`) if needed.

## Code quality bar

This codebase should stay easy to read, change, and extend without a rewrite. On every change:

- **SOLID where it earns its keep.** Apply SRP, DIP, etc. when they reduce coupling or make the next
  change cheaper — not as a checklist. Don't introduce an interface, abstraction, or extra layer for a
  single implementation that has no second caller and no test-isolation need. Prefer the boring, direct
  solution unless there's a concrete reason (multiple implementations, need to mock in tests, genuine
  variability) to abstract.
- **No code smells:** no God classes/methods, no deep nesting, no duplicated logic (extract instead of
  copy-paste), no long parameter lists, no magic numbers/strings, no dead code, no swallowed exceptions.
- **Readability first.** Descriptive names over comments. A reader should understand *what* from the
  code; comments (sparingly) explain *why* only when it's non-obvious (a workaround, an invariant, a
  constraint from another service).
- **Match existing patterns.** Each service already follows Api/Contracts/Domain/EntityFramework
  layering — put new code in the layer it belongs to, don't shortcut across layers (e.g. no EF types
  leaking into Contracts, no domain logic in controllers).
- **Small, focused changes.** Don't refactor unrelated code or add speculative abstractions/config for
  hypothetical future requirements while doing an unrelated task.

## Testing

- Every service with business logic has a matching `*.Tests` project (xUnit + coverlet). New domain
  logic, API endpoints, and bug fixes need tests in the corresponding project — don't leave a
  service-with-tests without coverage for new code.
- Test behavior (inputs → outputs / state changes), not implementation details, so tests survive
  refactors.
- Cover the edge cases and failure paths, not just the happy path.
- Angular: components/services with logic get Jasmine specs (`ng test`) alongside the existing pattern
  in `Unmatched.UI.Angular/src`.
- Before calling backend work done, run the relevant test project, e.g.:
  ```
  dotnet test Services/Catalog/Unmatched.CatalogService.Tests/Unmatched.CatalogService.Tests.csproj
  ```
  or `dotnet test Unmatched.sln` for the whole solution.

## Conventions

- `Nullable` and `ImplicitUsings` are enabled — don't suppress nullable warnings, fix the actual null
  path.
- Follow `Unmatched.UI.Angular/.editorconfig` for frontend formatting.
- New services follow the same project layout as the existing four (`Api`/`Contracts`/`Domain`/
  `EntityFramework`/`Tests`) unless there's a stated reason to deviate.
