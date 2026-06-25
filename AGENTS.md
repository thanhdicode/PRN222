# AGENTS.md — Operating Rules for AI Coding Agents

This file is the highest-priority project instruction file for all coding agents.

## Mission

Build a PRN222 big assignment named **Manga Creation Workflow and Publishing Management System** using ASP.NET Core and SQL Server. The project must be complete enough to demo all major PRN222 concepts without overengineering.

## Course alignment is mandatory

Every implemented feature must help demonstrate at least one of these PRN222 areas:

1. C# / .NET fundamentals.
2. EF Core with SQL Server.
3. Networking / HTTP / optional mock API call.
4. Asynchronous and parallel programming.
5. Dependency Injection.
6. ASP.NET Core MVC.
7. ASP.NET Core Razor Pages.
8. Blazor dashboard/component.
9. SignalR realtime communication.
10. Worker Service / Hosted Service background task.

If a requested task does not support one of these areas, ask whether it should be postponed.

## Core product scope

The app manages manga production workflow:

- Mangaka creates series, chapters, manuscripts, manga pages.
- Mangaka selects page regions and assigns production tasks to assistants.
- Assistant views assigned tasks and uploads submissions.
- Mangaka reviews submissions and approves/rejects/requests revision.
- Tantou Editor annotates manga pages and monitors progress.
- Editorial Board votes on series, chooses schedule, imports reader vote data, and reviews rankings.
- System sends realtime notifications with SignalR.
- Worker service scans deadlines, overdue tasks, ranking risk, notification cleanup, and assistant earnings.

## Do not overengineer

Do **not** add these unless explicitly requested:

- Microservices.
- Kubernetes.
- Cloud architecture.
- CQRS/MediatR/event sourcing.
- Clean Architecture with too many projects.
- Payment gateway.
- Mobile app.
- Real AI model training.
- Complex DDD aggregates.
- Docker unless needed for local convenience.
- Advanced authorization policies beyond roles.
- Unit-of-work abstraction if EF Core DbContext already handles it.

Use a layered architecture because PRN222 labs/assignments use DAO/Repository/Service style, but keep it understandable for students.

## Recommended solution structure

Preferred full PRN222 structure:

```text
MangaWorkflowSystem/
├── MangaWorkflow.BusinessObjects
├── MangaWorkflow.DataAccessObjects
├── MangaWorkflow.Repositories
├── MangaWorkflow.Services
├── MangaWorkflow.Web
└── MangaWorkflow.Worker
```

Simplified fallback if time is limited:

```text
MangaWorkflowSystem/
├── MangaWorkflow.Web
└── MangaWorkflow.Worker
```

Inside `MangaWorkflow.Web`, keep folders for `Controllers`, `Pages`, `Views`, `Components`, `Hubs`, `wwwroot`, and view models.

## Database rule

Use the supplied SQL Server scripts:

1. Run `db/MangaWorkflowDB_v2_demo_ready.sql`.
2. Run `db/MangaWorkflowDB_v3_extra_seed_demo_data.sql`.

Use database-first EF Core scaffolding unless the team explicitly switches to code-first. Do not change table names casually because the docs, seed data, and demo flow depend on them.

## Coding rules

- Use async methods for EF Core calls: `ToListAsync`, `FindAsync`, `FirstOrDefaultAsync`, `SaveChangesAsync`.
- Avoid `.Result` and `.Wait()` in request-handling code.
- Register services and repositories with Dependency Injection in `Program.cs`.
- Controllers/PageModels should be thin. Business rules go into Services.
- Repositories/DAO should only handle data access, not workflow decisions.
- Validate uploaded files by extension, content type, and size.
- Do not hardcode user IDs. Use logged-in user context later; for early demo, use seed accounts with clear TODO comments.
- Use Bootstrap for UI unless the team already chose another library.

## Required demo route coverage

Before considering the project complete, these screens must work:

- Login or role switch demo.
- Series list and details.
- Create/edit/submit series.
- Chapter and manga page list.
- Page region/task assignment.
- Assistant task inbox.
- Submission upload.
- Mangaka submission review.
- Editor comment/annotation list.
- Board voting screen.
- Reader vote/ranking screen.
- Blazor dashboard.
- SignalR notification dropdown or live feed.
- Background job log screen.

## Definition of Done

A feature is done only when:

1. It has a working UI.
2. It uses service/repository or service/DAO, not direct heavy logic in UI.
3. It reads/writes SQL Server data.
4. It handles invalid input gracefully.
5. It has at least one demo seed data path.
6. It does not break the final demo flow.
7. It is mentioned in README or demo playbook if user-facing.

## Agent handoff rule

At the end of every coding session, update:

- `docs/PROJECT_STATUS.md`
- pending TODOs
- known errors
- exact commands run
- next recommended action
