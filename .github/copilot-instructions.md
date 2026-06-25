# GitHub Copilot Instructions — MangaWorkflow PRN222

You are helping build a PRN222 ASP.NET Core big assignment, not a production SaaS.

Read `AGENTS.md` and `docs/00_PROJECT_BRIEF.md` before making architectural suggestions.

## Project stack

- ASP.NET Core .NET 8+.
- SQL Server 2019+.
- EF Core database-first.
- MVC for management modules.
- Razor Pages for assistant workflows.
- Blazor for dashboard.
- SignalR for realtime notification.
- Worker Service/Hosted Service for background jobs.
- DI + async/await + service/repository pattern.

## Scope rules

Do not suggest microservices, CQRS, event sourcing, Kubernetes, payment, mobile, React SPA, or real AI model training. Optional AI segmentation is mock/API integration only.

## Coding style

- Keep controllers thin.
- Use services for business rules.
- Use async EF Core calls.
- Use constants for status codes.
- Do not hardcode status IDs.
- Validate user input.
- Keep UI simple with Bootstrap.

## Required features

The finished app must demonstrate:

- Series/chapter/page CRUD.
- Page region + task assignment.
- Assistant task inbox and submission.
- Mangaka review.
- Editor comments.
- Board voting.
- Ranking data.
- Notifications.
- Blazor dashboard.
- Worker logs.
