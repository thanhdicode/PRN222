# Architectural Refactoring: To Domain, Application, Infrastructure

## Objective
This document outlines the architectural refactoring of the Manga Creation Workflow and Publishing Management System. The project was initially scaffolded using a simplified n-tier structure (BusinessObjects, DataAccessObjects, Services) and has now been refactored into a Clean Architecture-aligned structure (Domain, Application, Infrastructure).

## Summary of Changes

### 1. MangaWorkflow.BusinessObjects -> MangaWorkflow.Domain
- Moved entities, enums, and constants to the `Domain` layer.
- Updated namespaces to `MangaWorkflow.Domain.Entities`.

### 2. MangaWorkflow.Services -> MangaWorkflow.Application
- The `Application` layer now contains all application logic, interfaces, DTOs, and view models.
- **Interfaces**: Moved all repository interfaces (`MangaWorkflow.Repositories.Interfaces`) to `MangaWorkflow.Application.Interfaces.Repositories`.
- **Services**: Service implementations and interfaces are now housed here.

### 3. MangaWorkflow.DataAccessObjects & Repositories -> MangaWorkflow.Infrastructure
- Consolidated data access and repository implementations into the `Infrastructure` layer.
- `MangaWorkflowDbContext` is now located in `MangaWorkflow.Infrastructure.Persistence`.
- Repository implementations are now located in `MangaWorkflow.Infrastructure.Repositories`.

### 4. Dependency Injection
- Created `DependencyInjection.cs` in both the `Application` and `Infrastructure` layers.
- `Program.cs` in the `Web`, `Worker`, and `Tools.DbSmokeTest` projects now call `AddApplicationServices()` and `AddInfrastructure(builder.Configuration)` instead of manually registering each service.

## Project Structure
```text
MangaWorkflowSystem/
├── MangaWorkflow.Domain/           # Entities, Enums, Constants
├── MangaWorkflow.Application/      # DTOs, Services, Interfaces (Repositories & Services)
├── MangaWorkflow.Infrastructure/   # DbContext, Repository Implementations
├── MangaWorkflow.Web/              # Blazor UI, Controllers, SignalR
├── MangaWorkflow.Worker/           # Background services
└── MangaWorkflow.Tools.DbSmokeTest/# Console app for testing
```

## Maintenance
- The `Application` layer should NEVER reference the `Infrastructure` layer.
- The `Infrastructure` layer implements interfaces defined in the `Application` layer.
- The `Domain` layer should not depend on any other layer in the solution.
