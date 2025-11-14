# MiniDashboard

Sample application for Daifuku

Built with .Net 10 in Visual Studio 2026


## Solution structure

| Project | Description |
| --- | --- |
| `MiniDashboard.Context` | Shared domain layer that contains the Entity Framework Core entities, transport DTOs, and service interfaces consumed by both the API and the WPF app. |
| `MiniDashboard.Api` | ASP.NET Core application that exposes REST endpoints for authors and books, backed by EF Core and the shared contracts. |
| `MiniDashboard.App` | WPF desktop client that follows the MVVM pattern and talks to the API through the shared service abstractions. |
| `MiniDashboard.Tests.UnitTests` | Unit tests targeting the MVVM view models and service logic by substituting the shared interfaces. |
| `MiniDashboard.Tests.IntegrationTests` | Integration tests that spin up the API with an in-memory database to exercise the HTTP endpoints. |

## Layer interactions

```
WPF Views ─┐
           ├──> ViewModels (MiniDashboard.App) ──> IAuthorService / IBooksService ──┐
API Client ┘                                                                        │
                                                                                    ▼
                                                                    Shared DTOs & Interfaces
                                                                     (MiniDashboard.Context)
                                                                                │
                                                                                ▼
                                                         ASP.NET Core Controllers & Services
                                                               (MiniDashboard.Api)
                                                                                │
                                                                                ▼
                                                          EF Core DbContext & Entities
```

### Shared context
- DTOs such as `BookDto` bridge between persistence entities and client-side models while retaining helper projections for display (`AuthorString`, `GenreString`).
- Interfaces like `IBooksService`/`IAuthorService` define the contract that both the API services and the WPF HTTP clients implement, keeping the app loosely coupled to the transport mechanism.
- Entity classes inside `ApiModels` back the EF Core `ApiDbContext` used by the web application.

### Web API
- `Program.cs` wires up controllers, OpenAPI/Scalar documentation, EF Core (SQLite in development), and registers the context-aware service implementations (`AuthorService`, `BooksService`).
- Controllers (`BookController`, `AuthorController`) expose CRUD and search endpoints that delegate to the shared service interfaces.
- Service classes translate DTOs to entities, manage relationships (authors/genres), and persist changes through EF Core.

### WPF application
- `App.xaml.cs` bootstraps a dependency injection container, configures an `HttpClient` pointed at the API, and registers view models plus service implementations that call the HTTP endpoints.
- View models (e.g. `BooksViewModel`, `AuthorsViewModel`) coordinate UI state, commands, and data loading/saving logic via the injected services.
- Services in `MiniDashboard.App.Services` implement the shared interfaces by issuing HTTP requests (`GetAllAsync`, `CreateBookAsync`, etc.) against the API, isolating network access from the UI.

### Automated tests
- Unit tests substitute the shared service interfaces with `NSubstitute` to validate view model behaviour (command enablement, dialog workflows) and service logic using in-memory EF Core contexts.
- Integration tests rely on a custom `WebApplicationFactory` that replaces the database with an in-memory provider and seeds sample data before running end-to-end HTTP assertions against `/books` and `/authors` endpoints.

## Running the solution locally

1. **Restore and build**
   ```bash
   dotnet restore MiniDashboard.slnx
   dotnet build MiniDashboard.slnx
   ```
2. **Run the API**
   ```bash
   dotnet run --project MiniDashboard.Api
   ```
   The API listens port 5273 by default and uses a local SQLite database (`minidashboard.sqlite`).
3. **Run the WPF client**
   ```bash
   dotnet run --project MiniDashboard.App
   ```
   Ensure the API is running so the client can retrieve and persist data.
4. **Execute tests**
   ```bash
   dotnet test MiniDashboard.slnx
   ```
   Unit tests and integration tests will execute together as part of the solution.

## Database migrations

The API project contains Entity Framework Core migrations. To update or recreate the SQLite database, use the standard EF Core tooling, for example:
```bash
dotnet ef database update --project MiniDashboard.Api
```