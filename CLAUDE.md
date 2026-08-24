# TicketFlow — Web API

## Project Context

TicketFlow is a .NET 10 REST API built with **DDD + Clean Architecture**. The domain is expected to carry rich business rules (ticket lifecycle, assignment, workflow state), so the domain model uses tactical DDD patterns (aggregates, value objects, domain events) inside a Clean Architecture layering. Solo-developer project, single deployable service (no plans for modular monolith / microservices split at this stage).

## Tech Stack

- **.NET 10** / C# 14
- **ASP.NET Core Minimal APIs** — `IEndpointGroup` per feature with `app.MapEndpoints()` auto-discovery
- **Entity Framework Core** — **MySQL via Pomelo.EntityFrameworkCore.MySql** (see `docker-compose.yml` for local MySQL 8.4)
- **JWT bearer authentication** — stateless token auth (not yet implemented — see `authentication` skill)
- **Mediator** (source-generated, MIT) or raw handlers — command/query dispatch orchestrating aggregates
- **FluentValidation** — request validation
- **Serilog** — structured logging
- **xUnit v3** + **Testcontainers** — testing (mirror the MySQL container from `docker-compose.yml` in integration tests)

## Architecture

DDD + Clean Architecture. Current layout (already scaffolded):

```
src/
  TicketFlow.Domain/         # Aggregates, value objects, domain events, domain services (no dependencies)
  TicketFlow.Application/    # Use cases orchestrating aggregates (references Domain)
  TicketFlow.Infrastructure/ # EF Core (MySQL/Pomelo), external service adapters (references Application + Domain)
  TicketFlow.Api/            # Thin minimal API endpoints, JWT auth (references Application + Infrastructure)
tests/
  TicketFlow.UnitTests/         # Domain and application logic, no external dependencies
  TicketFlow.IntegrationTests/  # WebApplicationFactory + Testcontainers (MySQL)
```

Dependency direction is enforced via project references — `Domain` has zero dependencies, everything points inward toward it. Run the `arch-check` skill periodically to verify no layer violations creep in.

**Cleanup still pending:** `Class1.cs` stub files in Domain/Application/Infrastructure and the default `WeatherForecastController`/`WeatherForecast.cs` in Api need to be removed once the first real feature is scaffolded.

## Coding Standards

- **C# 14 features** — Use primary constructors, collection expressions, `field` keyword, records, pattern matching
- **File-scoped namespaces** — Always
- **`var` for obvious types** — Use explicit types when the type isn't clear from context
- **Naming** — PascalCase for public members, `_camelCase` for private fields, suffix async methods with `Async`
- **No regions** — Ever
- **No comments for obvious code** — Only comment "why", never "what"
- **DDD conventions** — Aggregate roots expose behavior methods, not public setters; value objects are immutable records; domain events are raised inside aggregates and dispatched after `SaveChangesAsync`

## Skills

Load these dotnet-claude-kit skills for context:

- `modern-csharp` — C# 14 language features and idioms
- `clean-architecture` — Layered project structure with dependency inversion
- `ddd` — Aggregates, value objects, domain events (pair with `clean-architecture`)
- `arch-check` — Verify dependency direction and layer boundaries stay clean
- `minimal-api` — Endpoint routing, TypedResults, OpenAPI metadata
- `ef-core` — DbContext patterns, query optimization, migrations (MySQL/Pomelo specifics)
- `testing` — xUnit v3, WebApplicationFactory, Testcontainers
- `error-handling` — Result pattern, ProblemDetails
- `authentication` — JWT bearer setup
- `logging` — Serilog, OpenTelemetry
- `configuration` — Options pattern, secrets management (connection strings via user-secrets, `UserSecretsId` already set in `TicketFlow.Api.csproj`)
- `dependency-injection` — Service registration patterns
- `scaffold` — Generate complete feature slices (endpoint, handler, validator, DTOs, EF config, tests) matching this architecture
- `workflow-mastery` — Parallel worktrees, verification loops, subagent patterns, context discipline
- `instinct-system` — Capture corrections, instincts, and discoveries as persistent learning

## MCP Tools

> **Setup:** Install once globally with `dotnet tool install -g CWM.RoslynNavigator` and register with `claude mcp add --scope user cwm-roslyn-navigator -- cwm-roslyn-navigator --solution ${workspaceFolder}`. After that, these tools are available in every .NET project.

Use `cwm-roslyn-navigator` tools to minimize token consumption:

- **Before modifying a type** — Use `find_symbol` to locate it, `get_public_api` to understand its surface
- **Before adding a reference** — Use `find_references` to understand existing usage
- **To understand architecture** — Use `get_project_graph` to see project dependencies
- **To find implementations** — Use `find_implementations` instead of grep for interface/abstract class implementations
- **To check for errors** — Use `get_diagnostics` after changes

## Commands

```bash
# Build
dotnet build

# Run (development)
dotnet run --project src/TicketFlow.Api

# Start local MySQL
docker compose up -d

# Run tests
dotnet test

# Add EF migration
dotnet ef migrations add [Name] --project src/TicketFlow.Infrastructure --startup-project src/TicketFlow.Api

# Apply migrations
dotnet ef database update --project src/TicketFlow.Infrastructure --startup-project src/TicketFlow.Api

# Format check
dotnet format --verify-no-changes
```

## Workflow

- **Plan first** — Enter plan mode for any non-trivial task (3+ steps or architecture decisions). Iterate until the plan is solid before writing code.
- **Verify before done** — Run `dotnet build` and `dotnet test` after changes. Use `get_diagnostics` via MCP to catch warnings. Ask: "Would a staff engineer approve this?"
- **Fix bugs autonomously** — When given a bug report, investigate and fix it without hand-holding. Check logs, errors, failing tests — then resolve them.
- **Stop and re-plan** — If implementation goes sideways, STOP and re-plan. Don't push through a broken approach.
- **Use subagents** — Offload research, exploration, and parallel analysis to subagents. One task per subagent for focused execution.
- **Learn from corrections** — After any correction, capture the pattern in memory so the same mistake never recurs.

## Anti-patterns

Do NOT generate code that:

- Defines endpoints in Program.cs — use `IEndpointGroup` per feature with `app.MapEndpoints()` auto-discovery
- Manually wires MapGroup calls in Program.cs — Program.cs should never change when adding endpoints
- Puts business logic in Application handlers instead of the aggregate — handlers orchestrate, aggregates decide
- Exposes public setters on aggregate roots or entities — mutate only through behavior methods
- Uses primitive obsession for domain concepts (raw `Guid`/`string` for IDs or statuses) — use strongly-typed IDs and value objects
- Uses `DateTime.Now` — use `TimeProvider` injection instead
- Creates `new HttpClient()` — use `IHttpClientFactory`
- Uses `async void` — always return `Task`
- Blocks with `.Result` or `.Wait()` — await instead
- Uses `Results.Ok()` — use `TypedResults.Ok()` for OpenAPI
- Returns domain entities from endpoints — always map to response DTOs
- Creates repository abstractions over EF Core unless justified by aggregate persistence boundaries — use DbContext directly for simple reads
- Uses in-memory database for tests — use Testcontainers (MySQL)
- Catches bare `Exception` — catch specific types, let the global handler catch the rest
- Uses string interpolation in log messages — use structured logging templates
