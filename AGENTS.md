# Repository Guidelines

## Project Structure & Module Organization

This is a .NET 6 hotel booking API organized in `Hotel.Booking.sln`.

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
