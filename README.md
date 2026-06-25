# PRN222 MangaWorkflow Fullstack Documentation Pack

Project: **Manga Creation Workflow and Publishing Management System**  
Course: **PRN222 - Advanced Cross-Platform Application Programming With .NET**  
Purpose: provide a complete, agent-readable context pack so any AI coding agent can continue the project without guessing scope, architecture, features, database intent, or PRN222 requirements.

## Start here

Read these files in order:

1. `AGENTS.md` — global rules for every AI agent.
2. `docs/00_PROJECT_BRIEF.md` — what the project is and what it is not.
3. `docs/01_SRS.md` — full software requirements specification.
4. `docs/02_PRN222_ALIGNMENT_MATRIX.md` — how every course chapter maps to implementation.
5. `docs/03_DOMAIN_MODEL_DATABASE_GUIDE.md` — SQL Server database guide and table responsibilities.
6. `docs/04_ARCHITECTURE_PLAYBOOK.md` — solution structure, layering, dependency rules.
7. `docs/05_IMPLEMENTATION_PLAYBOOK.md` — exact build order for coding agents.
8. `docs/11_DEMO_PRESENTATION_PLAYBOOK.md` — final demo flow for grading.

## Included artifacts

- `db/MangaWorkflowDB_v2_demo_ready.sql` — schema + base demo data.
- `db/MangaWorkflowDB_v3_extra_seed_demo_data.sql` — extra seed data for coding and demo.
- `.github/copilot-instructions.md` — GitHub Copilot project instructions.
- `.cursor/rules/manga-prn222.mdc` — Cursor/Cline-compatible project rule.
- `.continue/rules/manga-prn222.md` — Continue rule/check style context.
- `prompts/` — reusable prompts for Codex, Cline, Cursor, Copilot, Claude Code, Gemini CLI, Aider, etc.

## System Architecture

The solution uses a Clean Architecture-inspired structure tailored for PRN222 requirements:

*   **MangaWorkflow.Domain**: Core business entities, enums, and constants.
*   **MangaWorkflow.Application**: Application logic, interfaces (Repositories, Services), DTOs, and ViewModels.
*   **MangaWorkflow.Infrastructure**: Data access implementations (EF Core DbContext) and Repository implementations.
*   **MangaWorkflow.Web**: The ASP.NET Core web application containing MVC Controllers, Blazor components (Server-side), SignalR Hubs, and Views.
*   **MangaWorkflow.Worker**: A .NET Background Worker Service for asynchronous tasks (e.g., polling overdue tasks).
*   **MangaWorkflow.Tools.DbSmokeTest**: A simple console application for verifying database connectivity and basic EF Core queries.

## Non-negotiable course fit

This is a PRN222 big assignment. The completed app must visibly demonstrate:

- ASP.NET Core MVC.
- ASP.NET Core Razor Pages.
- Blazor dashboard/component.
- SignalR realtime notification/workflow updates.
- Worker Service or Hosted Service background tasks.
- EF Core with SQL Server.
- Dependency Injection.
- Async/await and basic parallel programming where useful.
- Team-oriented layered architecture: BusinessObjects, DataAccessObjects, Repositories, Services, Web/Worker.

## One-sentence project definition

MangaWorkflow is a role-based ASP.NET Core web application for managing manga studio workflow from series proposal, manuscript/page upload, region-based task assignment, assistant submission, mangaka review, editor annotation, board voting, publication issue tracking, reader ranking, realtime notification, and background deadline/ranking automation.

## Hard scope limit

Do not build a manga reader, drawing app, marketplace, payment system, mobile app, cloud deployment platform, or real deep-learning training pipeline. Optional AI segmentation is a mock/API-integration module only.
