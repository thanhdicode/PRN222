# PRN222 MangaWorkflow Fullstack Documentation Pack

Project: **Manga Creation Workflow and Publishing Management System**
Course: **PRN222 - Advanced Cross-Platform Application Programming With .NET**
Purpose: Complete, agent-readable context pack so any AI coding agent can continue the project without guessing scope, architecture, features, database intent, or PRN222 requirements.

---

## Quick Start

### Prerequisites
- .NET 8 SDK or later
- SQL Server (local or LocalDB)
- Run database scripts (see Database Setup below)

### Database Setup

Run these SQL scripts against your SQL Server in this order:
1. `db/MangaWorkflowDB_v2_demo_ready.sql`
2. `db/MangaWorkflowDB_v3_extra_seed_demo_data.sql`

Verify:
```sql
SELECT COUNT(*) FROM dbo.Users;
SELECT COUNT(*) FROM dbo.Series;
SELECT COUNT(*) FROM dbo.ProductionTasks;
SELECT * FROM dbo.vw_ChapterProgress;
```

### appsettings.json

Update connection string in MangaWorkflow.Web/appsettings.json:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=MangaWorkflowDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

For LocalDB, use:
`Server=(localdb)\\MSSQLLocalDB;Database=MangaWorkflowDB;Trusted_Connection=True;TrustServerCertificate=True;`

### Run the Application

```bash
dotnet clean
dotnet restore
dotnet build
dotnet run --project MangaWorkflow.Tools.DbSmokeTest
dotnet run --project MangaWorkflow.Web
```

For the Worker Service (Phase 5):
```bash
dotnet run --project MangaWorkflow.Worker
```

### Demo Accounts

| Email | Role | Notes |
|---|---|---|
| admin@manga.local | Admin | Full access |
| mangaka@manga.local | Mangaka | Manga creator |
| assistant@manga.local | Assistant | Production worker |
| editor@manga.local | Editor (Tantou) | Page annotation |
| board@manga.local | EditorialBoard | Series voting and ranking |

Passwords: See database seed data. Demo mode uses stored passwords.

---

## Project Status

**Current Phase**: Phase 2 — ASP.NET Core MVC CRUD (NEXT TO IMPLEMENT)

| Phase | Status |
|---|---|
| Phase 0: Database | ✅ Done |
| Phase 1: Foundation, DI, EF Core | ✅ Done |
| Phase 2: ASP.NET Core MVC CRUD | ⏳ Next |
| Phase 3: Razor Pages Workflow | 🔒 Locked |
| Phase 4: Blazor + SignalR | 🔒 Locked |
| Phase 5: Worker + Final Demo | 🔒 Locked |

See docs/PROJECT_STATUS.md for detailed status.

---

## Solution Structure

```
MangaWorkflowSystem/
├── MangaWorkflow.Domain/           # EF Core entities, enums, constants
├── MangaWorkflow.Application/      # Services, Interfaces, DTOs, ViewModels
├── MangaWorkflow.Infrastructure/   # DbContext, Repository implementations
├── MangaWorkflow.Web/              # MVC, Razor Pages, Blazor, SignalR, wwwroot
├── MangaWorkflow.Worker/           # Background Worker Services
└── MangaWorkflow.Tools.DbSmokeTest/# Database connection test console app
```

### Dependency Direction
```
Domain <- Application <- Infrastructure
Domain <- Application <- Web
Domain <- Application <- Worker
Domain <- Infrastructure <- Web/Worker
```

---

## AI Agent Instructions

If you are an AI coding agent, read these files in order:
1. `AGENTS.md` — global rules for every AI agent
2. `docs/AI_AGENT_EXECUTION_RULES.md` — hard rules before writing code
3. `docs/MASTER_AGENT_PLAN.md` — overall phase plan
4. `docs/PROJECT_STATUS.md` — what is done and what is next
5. `docs/PHASE_2_MVC_CRUD_PLAN.md` — detailed guide for Phase 2

To implement Phase 2, copy and use:
`prompts/PHASE_2_IMPLEMENTATION_PROMPT.md`

---

## Document Index

| File | Purpose |
|---|---|
| AGENTS.md | Global AI agent rules |
| docs/AI_AGENT_EXECUTION_RULES.md | Hard execution rules for coding agents |
| docs/MASTER_AGENT_PLAN.md | Master 5-phase plan |
| docs/PHASE_2_MVC_CRUD_PLAN.md | Phase 2 detailed implementation guide |
| docs/PHASE_3_RAZOR_PAGES_WORKFLOW_PLAN.md | Phase 3 detailed guide |
| docs/PHASE_4_BLAZOR_SIGNALR_PLAN.md | Phase 4 detailed guide |
| docs/PHASE_5_WORKER_FINAL_DEMO_PLAN.md | Phase 5 detailed guide |
| docs/AI_SKILLS_AND_REPO_TOOLS.md | Recommended AI tools for development |
| docs/00_PROJECT_BRIEF.md | What the project is |
| docs/01_SRS.md | Full software requirements |
| docs/02_PRN222_ALIGNMENT_MATRIX.md | Course chapter to feature mapping |
| docs/03_DOMAIN_MODEL_DATABASE_GUIDE.md | Database tables, views, coding rules |
| docs/04_ARCHITECTURE_PLAYBOOK.md | Layer responsibilities |
| docs/05_IMPLEMENTATION_PLAYBOOK.md | Original step-by-step coding plan |
| docs/09_SIGNALR_WORKER_PLAYBOOK.md | SignalR hub and Worker reference |
| docs/11_DEMO_PRESENTATION_PLAYBOOK.md | Final demo flow for grading |
| docs/14_SCOPE_CONTROL_NO_OVERENGINEERING.md | Feature kill criteria |
| docs/PROJECT_STATUS.md | Live project status tracking |
| prompts/PHASE_2_IMPLEMENTATION_PROMPT.md | Ready-to-copy Phase 2 agent prompt |
| prompts/PHASE_3_IMPLEMENTATION_PROMPT.md | Ready-to-copy Phase 3 agent prompt |
| prompts/PHASE_4_IMPLEMENTATION_PROMPT.md | Ready-to-copy Phase 4 agent prompt |
| prompts/PHASE_5_IMPLEMENTATION_PROMPT.md | Ready-to-copy Phase 5 agent prompt |
| db/MangaWorkflowDB_v2_demo_ready.sql | Schema + base demo data |
| db/MangaWorkflowDB_v3_extra_seed_demo_data.sql | Extra seed data |

---

## PRN222 Demonstration Coverage

| PRN222 Chapter | Implementation | Phase |
|---|---|---|
| Ch01: .NET Fundamentals | Entity models, services, LINQ | Phase 1 |
| Ch02: Async/Parallel | All EF Core calls async, Task.WhenAll for notifications | Phase 1+ |
| Ch03: Dependency Injection | Repository+Service DI registration | Phase 1 |
| Ch04: ASP.NET Core MVC | Areas, Controllers, Views, CRUD, Search, Filter | Phase 2 |
| Ch05: Razor Pages | PageModel, OnGetAsync, OnPostAsync, workflow screens | Phase 3 |
| Ch06: Blazor | Server-side Blazor dashboards with real data | Phase 4 |
| Ch07: SignalR | Hubs, real-time notification events, JS client | Phase 4 |
| Ch08: Worker Service | BackgroundService, timed loops, BackgroundJobLogs | Phase 5 |

---

## One-Sentence Project Definition

MangaWorkflow is a role-based ASP.NET Core web application for managing manga studio workflow from series proposal, manuscript/page upload, region-based task assignment, assistant submission, mangaka review, editor annotation, board voting, publication issue tracking, reader ranking, realtime notification, and background deadline/ranking automation.

---

## Hard Scope Limits

Do not build: manga reader, drawing app, marketplace, payment system, mobile app, cloud deployment, real deep-learning training pipeline.

Do not use: CQRS, MediatR, microservices, Docker, Redis, RabbitMQ, React, Angular, GraphQL.

This is a PRN222 academic assignment. The goal is demonstrating course concepts, not enterprise SaaS.
