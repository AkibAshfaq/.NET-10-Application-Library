# Job Portal (bdjobs-style) — DDD in ASP.NET Core 8 / SQL Server

End-to-end build guide. Follow phases in order — each one produces a working, testable slice.

## 0. Prerequisites

- .NET 8 SDK, SQL Server (LocalDB or Docker), EF Core CLI (`dotnet tool install --global dotnet-ef`)
- IDE: Visual Studio, Rider, or VS Code + C# Dev Kit

## 1. Solution & Project Layout

Create the solution with four layers. Dependencies flow inward — Domain has zero references.

```
JobPortal.sln
src/
  JobPortal.Domain/            # Entities, Value Objects, Aggregates, Domain Events, Interfaces
  JobPortal.Application/       # Use cases (CQRS), DTOs, validators, application interfaces
  JobPortal.Infrastructure/    # EF Core, repositories, external services (email, file storage)
  JobPortal.Api/               # Controllers/Minimal APIs, DI wiring, middleware
tests/
  JobPortal.Domain.Tests/
  JobPortal.Application.Tests/
  JobPortal.Api.Tests/         # integration tests (WebApplicationFactory)
```

Commands:

```bash
dotnet new sln -n JobPortal
dotnet new classlib -n JobPortal.Domain -o src/JobPortal.Domain
dotnet new classlib -n JobPortal.Application -o src/JobPortal.Application
dotnet new classlib -n JobPortal.Infrastructure -o src/JobPortal.Infrastructure
dotnet new webapi -n JobPortal.Api -o src/JobPortal.Api

dotnet sln add src/**/*.csproj

dotnet add src/JobPortal.Application reference src/JobPortal.Domain
dotnet add src/JobPortal.Infrastructure reference src/JobPortal.Application
dotnet add src/JobPortal.Api reference src/JobPortal.Infrastructure
dotnet add src/JobPortal.Api reference src/JobPortal.Application
```

Note: Api references Infrastructure only for DI registration in `Program.cs`; controllers should depend on Application interfaces, not Infrastructure directly.

## 2. Identify Bounded Contexts / Aggregates

For a bdjobs-like portal, start with these aggregates (each gets its own folder under `Domain`):

- **Employer/Company** — `Company` (aggregate root), `CompanyProfile`, verification status
- **Recruiter/User (Identity)** — handled via ASP.NET Core Identity, kept thin; link via `UserId`
- **JobPosting** — `Job` (aggregate root) with `JobRequirements`, `SalaryRange` (value object), `JobStatus` (Draft/Published/Closed/Expired), owned by a `CompanyId`
- **CandidateProfile** — `Candidate` (aggregate root) with `Resume`, `Education`, `Experience`, `Skills`
- **Application** — `JobApplication` (aggregate root) linking `JobId` + `CandidateId`, with `ApplicationStatus` (Applied/Shortlisted/Interviewed/Rejected/Hired) and state-transition rules
- **Search/Category** — `Industry`, `JobCategory`, `Location` as reference/lookup entities (simpler, not full aggregates)

Keep aggregates small. `Job` should reference `CompanyId`, not hold the full `Company` object. `JobApplication` references `JobId`/`CandidateId` by ID only.

## 3. Domain Layer — build order

1. **Base building blocks** (`Domain/Common/`): `Entity<TId>` base class (Equals via Id), `AggregateRoot<TId>` (holds `DomainEvents` list), `ValueObject` base (structural equality), `IDomainEvent` marker interface.
2. **Value Objects**: `Email`, `Money`/`SalaryRange`, `Address`, `PhoneNumber` — validate invariants in constructor, throw domain exceptions on invalid input.
3. **Enums**: `JobStatus`, `ApplicationStatus`, `EmploymentType` (Full-time/Part-time/Remote/Contract), `ExperienceLevel`.
4. **Aggregate roots**, one at a time, starting with `Job`:
   - Private setters, constructors that enforce invariants, behavior methods instead of public setters (`job.Publish()`, `job.Close()`, not `job.Status = ...`).
   - Raise domain events on state changes (`JobPublishedEvent`, `ApplicationSubmittedEvent`).
5. **Repository interfaces** (`Domain/Repositories/`): `IJobRepository`, `ICandidateRepository`, etc. — return domain objects, no EF/SQL leakage.
6. **Domain services** for logic spanning aggregates (e.g. `IDuplicateApplicationChecker` — a candidate can't apply twice to the same job).

Write unit tests for invariants as you go (e.g. "closing an already-closed job throws", "applying with missing resume throws").

## 4. Application Layer — CQRS

Use MediatR for commands/queries; keeps controllers thin and use cases explicit.

```bash
dotnet add src/JobPortal.Application package MediatR
dotnet add src/JobPortal.Application package FluentValidation.DependencyInjectionExtensions
dotnet add src/JobPortal.Application package AutoMapper  # optional, or map manually
```

Structure per feature (vertical slices under `Application/Jobs/`, `Application/Applications/`, etc.):

```
Application/Jobs/
  Commands/
    CreateJob/  CreateJobCommand.cs, CreateJobCommandHandler.cs, CreateJobCommandValidator.cs
    PublishJob/ ...
  Queries/
    GetJobById/ GetJobByIdQuery.cs, GetJobByIdQueryHandler.cs, JobDto.cs
    SearchJobs/ SearchJobsQuery.cs (pagination, filters: location, category, salary range)
```

Each handler: load aggregate via repository interface → call domain method → persist via `IUnitOfWork.SaveChangesAsync()` → return DTO. Validators run automatically via a MediatR `ValidationBehavior` pipeline.

Define `IUnitOfWork`, `IApplicationDbContext` (if needed for read-only queries via EF directly, which is fine for queries — CQRS doesn't mean you can't query EF directly for reads).

## 5. Infrastructure Layer

```bash
dotnet add src/JobPortal.Infrastructure package Microsoft.EntityFrameworkCore.SqlServer
dotnet add src/JobPortal.Infrastructure package Microsoft.EntityFrameworkCore.Design
```

1. `JobPortalDbContext : DbContext, IApplicationDbContext` with `DbSet<Job>`, `DbSet<Company>`, etc.
2. **EF Core configurations** (`IEntityTypeConfiguration<Job>` per aggregate) in `Infrastructure/Persistence/Configurations/` — map value objects with `OwnsOne`, configure indexes (e.g. index on `Job.Status`, `Job.CompanyId`, full-text or trigram index on `Job.Title` for search).
3. **Repository implementations** (`JobRepository : IJobRepository`) — wrap `DbContext`, translate domain queries to EF LINQ.
4. **Domain event dispatch**: override `SaveChangesAsync` to collect `DomainEvents` from tracked aggregates and publish them via MediatR after a successful save (outbox pattern is a later optimization).
5. Migrations:

```bash
dotnet ef migrations add InitialCreate -p src/JobPortal.Infrastructure -s src/JobPortal.Api
dotnet ef database update -p src/JobPortal.Infrastructure -s src/JobPortal.Api
```

## 6. API Layer

1. `Program.cs`: register DbContext, repositories, MediatR, FluentValidation, AutoMapper, ASP.NET Core Identity + JWT auth, Swagger.
2. Controllers per aggregate (`JobsController`, `ApplicationsController`, `CompaniesController`, `CandidatesController`) — each action just sends a MediatR command/query and returns the result. No business logic in controllers.
3. Auth: two roles minimum — `Employer` and `Candidate` (`Admin` later). Use `[Authorize(Roles = "Employer")]` on job-posting endpoints.
4. Global exception middleware mapping domain exceptions → proper HTTP status codes (404 for not found, 400 for validation/domain rule violations, 409 for conflicts like duplicate applications).
5. Swagger/OpenAPI for exploring the API as you build.

## 7. Suggested Build Order (milestones)

1. Scaffold solution + empty layers, confirm it builds (`dotnet build`), commit.
2. `Company` aggregate + registration/CRUD end-to-end (Domain → Application → Infra → API → test in Swagger).
3. `Job` aggregate: create/publish/close job, list/search jobs (public, no auth needed for browsing).
4. Identity/auth: Employer and Candidate registration/login, JWT issuance.
5. `Candidate` profile + resume upload (store resume as blob/file — use `IFileStorageService` abstraction, local disk first, swap for Azure Blob/S3 later).
6. `JobApplication` aggregate: apply to a job, prevent duplicates, employer views applicants, status transitions.
7. Cross-cutting: pagination/filtering on job search, logging (Serilog), FluentValidation everywhere, global error handling.
8. Testing: domain unit tests, application handler tests (in-memory/mocked repos), API integration tests with `WebApplicationFactory` + a test SQL Server (Testcontainers).
9. Nice-to-haves once core works: email notifications (job alerts, application status), CV parsing, recommendation/matching logic, admin moderation, rate limiting.

## 8. Guardrails to keep the design honest

- Domain project should have **no NuGet packages** except maybe a lightweight assertions/guard library — no EF, no ASP.NET references.
- Never expose EF entities directly from the API — always map to DTOs.
- Aggregates enforce their own invariants; if you're validating business rules in a controller or handler instead of inside the aggregate, that logic is in the wrong layer.
- One repository per aggregate root, not per table.
- Keep `JobPortalDbContext` in Infrastructure only; nothing above it should reference `Microsoft.EntityFrameworkCore` directly except for LINQ query composition in read-only query handlers if you choose that approach.

## 9. Reference reading

- Eric Evans, *Domain-Driven Design* (concepts)
- Vaughn Vernon, *Implementing Domain-Driven Design* (tactical patterns, closer to this stack)
- Microsoft's eShopOnContainers / eShop reference app — real ASP.NET Core DDD/CQRS example to compare against
