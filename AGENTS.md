# Repository Guidelines

## Project Structure & Module Organization

This is a .NET 10 hotel booking API organized in `Hotel.Booking.sln`.

- `src/Hotel.Booking.Api`: ASP.NET Core API, controllers, HTTP extensions, Swagger setup, and app settings.
- `src/Hotel.Booking.Core`: domain DTOs, entities, validation handlers, interfaces, AutoMapper profiles, and services.
- `src/Hotel.Booking.Infra`: Entity Framework InMemory context, seeds, and repository implementations.
- `tests/Hotel.Booking.UnitTest`: xUnit unit tests for services.
- `tests/Hotel.Booking.IntegrationTest`: integration test project scaffold.

Keep domain rules in `Core`, persistence in `Infra`, and HTTP concerns in `Api`.

## Build, Test, and Development Commands

Run commands from the repository root unless noted otherwise.

```powershell
dotnet restore
dotnet build Hotel.Booking.sln
dotnet test Hotel.Booking.sln
dotnet run --project src/Hotel.Booking.Api/Hotel.Booking.Api.csproj
```

- `dotnet restore` downloads dependencies.
- `dotnet build` compiles all projects in the solution.
- `dotnet test` runs tests.
- `dotnet run` starts the API; Swagger is available at `https://localhost:7209/swagger/index.html` when using the default launch profile.

## Coding Style & Naming Conventions

Use the existing C# style: four-space indentation, nullable references, implicit usings, PascalCase for public types and members, camelCase for locals and parameters, and `I`-prefixed interfaces.

Follow the current project naming pattern: `Hotel.Booking.<Layer>`. Keep classes focused on one concern, and avoid adding new dependencies unless the benefit is clear and documented.

## Testing Guidelines

Tests use xUnit, with Moq, AutoFixture, Shouldly, and coverlet in the unit test project. Place unit tests under `tests/Hotel.Booking.UnitTest` and mirror the production area, for example `Services/BookingServiceTests.cs`.

Name test classes after the class under test, using the `*Tests` suffix. Prefer direct business-rule tests: booking starts no earlier than the next day, stay length is limited to 3 days, and booking cannot be more than 30 days ahead.

## Commit & Pull Request Guidelines

Local Git history could not be inspected because Git flagged repository ownership as unsafe, so no project-specific commit convention was confirmed. Use concise, imperative commit messages such as `Add booking validation tests`.

Pull requests should include a short description, changed behavior, tests run, and manual validation steps. Link related issues when available. Include screenshots only for API documentation or UI-facing changes.

## Security & Configuration Tips

Do not commit secrets or machine-specific configuration. Keep environment-specific values in `appsettings.Development.json` or local environment variables. The current data store is EF Core InMemory, so do not treat local seeded data as persistent production state.

## Engineering Constitution

Treat security, performance, resilience, observability, high cohesion, low coupling, and low cyclomatic complexity as design criteria for every change. Apply them proportionally: improve the code for the risk in front of you, but do not add cache, queues, retries, locks, distributed tracing, or extra abstractions without evidence that they are needed.

- **Security:** never expose secrets, sensitive data, or internal implementation details in logs or responses. Validate input before persistence and prefer explicit domain failures for expected business errors.
- **Performance and resilience:** consider volume, cost, concurrency, provider translation, expected failures, and simplicity while designing the code. Choose database filtering, in-memory filtering, or hybrid flows based on the scenario, and document meaningful trade-offs.
- **Observability:** logs should explain relevant events without leaking sensitive data. Errors should keep enough context for diagnosis. Critical operations should consider structured logs, correlation IDs, or health checks when useful.
- **Trade-offs:** when security, performance, resilience, simplicity, and clarity conflict, record the decision, reason, alternatives considered, and residual risk in the change summary or PR.

## Engineering Quality Rules

Prioritize high cohesion, low coupling, and low cyclomatic complexity in every change.

- Keep business rules in `Hotel.Booking.Core`. Controllers should only handle HTTP concerns, and repositories should only handle persistence queries.
- Prefer small methods with one clear reason to change. If a method needs multiple nested branches, extract named private methods or domain validators.
- Avoid adding abstractions unless they reduce real duplication, isolate a dependency, or make a business rule easier to test.
- Do not let `Api` depend on persistence details. Do not put EF Core logic in controllers or domain validation in repositories.
- Keep cyclomatic complexity low by using guard clauses, early returns, and explicit domain methods instead of deeply nested `if/else`.
- New behavior must be covered by focused unit tests in `tests/Hotel.Booking.UnitTest`; endpoint behavior belongs in integration tests.
- Preserve public contracts unless the task explicitly includes a breaking change.
- After each implementation, include the suggested manual commit command in the final response. Do not execute the commit unless explicitly requested. Format: `git commit -m "Short imperative message"`.

Before implementing, ask:
1. Does this class still have one main responsibility?
2. Is this change depending only on the layer it should know?
3. Can the main behavior be tested without starting the API?
4. Did complexity decrease or stay contained?
