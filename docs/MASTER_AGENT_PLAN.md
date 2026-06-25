# MASTER AGENT PLAN — PRN222 MangaWorkflow System

**Project**: Manga Creation Workflow and Publishing Management System
**Course**: PRN222 — Advanced Cross-Platform Application Programming With .NET
**Architecture**: Domain / Application / Infrastructure / Web / Worker / Tools.DbSmokeTest
**Last Updated**: 2026-06-26
**Status**: Phase 1 Complete ? | Phase 2 Next ?

---

## Overview

This document is the highest-level plan for all AI coding agents working on this project.
Every coding agent MUST read this file AND `docs/AI_AGENT_EXECUTION_RULES.md` before writing a single line of code.

The project is designed specifically to satisfy PRN222 learning outcomes.
Each phase maps directly to one or more PRN222 course chapters.
Do not skip phases. Do not merge phases. Do not jump ahead.

---

## PRN222 Learning-Order Approach

The project MUST be built in chapter order because each phase proves a specific PRN222 competency:

| Phase | PRN222 Chapters Proven | Technology Used |
|---|---|---|
| Phase 1 ? | Ch01-Ch03: Foundation, EF Core, DI, Async | Domain + Application + Infrastructure skeleton, EF Core scaffold, DbSmokeTest |
| Phase 2 ? | Ch04: ASP.NET Core MVC | MVC Controllers, Views, Areas, Role Auth, CRUD, search, filter, validation |
| Phase 3 | Ch05: Razor Pages | Razor Pages, PageModel, OnGetAsync/OnPostAsync, workflow screens |
| Phase 4 | Ch06 Blazor + Ch07 SignalR | Blazor dashboards, SignalR hubs, real-time notification events |
| Phase 5 | Ch08: Worker Service + Final | BackgroundService workers, deadline/overdue/ranking jobs, final demo |

---

## Phase Status

| Phase | Name | Status | Evidence |
|---|---|---|---|
| Phase 0 | Database Ready | ? Done | MangaWorkflowDB seeded |
| Phase 1 | Foundation, Scaffold, DI | ? Done | Build passes, DbSmokeTest passes |
| Phase 2 | ASP.NET Core MVC CRUD | ? Next | Not started |
| Phase 3 | Razor Pages Workflow | ?? Locked until Phase 2 done | Not started |
| Phase 4 | Blazor + SignalR | ?? Locked until Phase 3 done | Not started |
| Phase 5 | Worker + Final Demo | ?? Locked until Phase 4 done | Not started |

---

## Phase 1 — Foundation (COMPLETE ?)

### What was proven
PRN222 Ch01-Ch03: .NET project structure, EF Core database-first scaffold, Dependency Injection, async/await pattern

### What was built
- 6-project solution: Domain, Application, Infrastructure, Web, Worker, Tools.DbSmokeTest
- EF Core database-first scaffold — entities in MangaWorkflow.Domain/Entities/
- MangaWorkflowDbContext in MangaWorkflow.Infrastructure/Persistence/
- Repository interfaces in MangaWorkflow.Application/Interfaces/Repositories/
- Repository implementations in MangaWorkflow.Infrastructure/Repositories/
- Service interfaces and basic implementations in MangaWorkflow.Application/
- DependencyInjection.cs in both Application and Infrastructure
- Program.cs in Web calls AddApplicationServices() and AddInfrastructure()
- DbSmokeTest console app verifies SQL Server connectivity

### Existing Infrastructure
- ISeriesRepository, IChapterRepository, IUserRepository, IProductionTaskRepository, INotificationRepository
- SeriesRepository, ChapterRepository, UserRepository, ProductionTaskRepository, NotificationRepository
- ISeriesService, IDashboardService, INotificationService
- SeriesService, DashboardService, NotificationService

### Completion evidence
- dotnet build passes with zero errors
- dotnet run --project MangaWorkflow.Tools.DbSmokeTest passes

---

## Phase 2 — ASP.NET Core MVC CRUD (NEXT ?)

### PRN222 Chapter Proven
Chapter 04: ASP.NET Core MVC — controllers, views, model binding, validation, filters, areas, role authorization

### Scope Summary
Build all MVC management screens inside Areas:
- Areas/Admin — User and Role management
- Areas/Mangaka — Series, Chapter, Manga Page CRUD
- Areas/Board — Series voting review + Reader ranking management

### Must NOT do in Phase 2
- No Razor Pages workflow screens (Phase 3)
- No Blazor dashboard (Phase 4)
- No SignalR UI (Phase 4)
- No Worker jobs (Phase 5)

### Full detail
See docs/PHASE_2_MVC_CRUD_PLAN.md

### Completion criteria
- Build passes, DbSmokeTest passes
- All Areas render without errors
- Full Mangaka create series — submit — Board vote flow works
- docs/PROJECT_STATUS.md updated with [DONE] Phase 2
- docs/PHASE_2_COMPLETION_REPORT.md created

---

## Phase 3 — Razor Pages Workflow Screens

### PRN222 Chapter Proven
Chapter 05: Razor Pages — PageModel, OnGetAsync, OnPostAsync, handler methods, TempData, form validation

### Scope Summary
- Pages/Assistant/ — Task inbox, task detail, submission upload
- Pages/Mangaka/ — Submission review, page region management
- Pages/Editor/ — Page comments and annotation

### Full detail
See docs/PHASE_3_RAZOR_PAGES_WORKFLOW_PLAN.md

---

## Phase 4 — Blazor Dashboard + SignalR Realtime

### PRN222 Chapters Proven
- Chapter 06: Blazor Server
- Chapter 07: SignalR

### Scope Summary
- Role-based Blazor dashboards
- NotificationHub and WorkflowHub SignalR hubs
- JavaScript notification client
- Real-time notification dropdown with toast

### Full detail
See docs/PHASE_4_BLAZOR_SIGNALR_PLAN.md

---

## Phase 5 — Worker Service + Final Integration + Demo

### PRN222 Chapter Proven
Chapter 08: Worker Service (BackgroundService)

### Scope Summary
- DeadlineReminderWorker
- OverdueTaskScannerWorker
- RankingRiskWorker
- NotificationCleanupWorker
- MonthlyEarningCalculatorWorker
- Full final demo script, QA checklist, final report outline

### Full detail
See docs/PHASE_5_WORKER_FINAL_DEMO_PLAN.md

---

## Architecture Invariants (Never Violate)

MangaWorkflow.Domain — no references to other projects
MangaWorkflow.Application — references Domain only, no DbContext access
MangaWorkflow.Infrastructure — references Domain + Application, implements interfaces
MangaWorkflow.Web — references all, Controllers/PageModels call Services only
MangaWorkflow.Worker — references all, workers use DI scope + services
MangaWorkflow.Tools.DbSmokeTest — references Domain + Infrastructure only

---

## Banned Technologies

CQRS, MediatR, Microservices, Docker, Redis, RabbitMQ, React, Angular, Vue, Next.js,
GraphQL, real AI/ML training, payment system, production identity server.

---

## Run Commands Reference

```
dotnet clean
dotnet restore
dotnet build
dotnet run --project MangaWorkflow.Tools.DbSmokeTest
dotnet run --project MangaWorkflow.Web
dotnet run --project MangaWorkflow.Worker   # Phase 5 only
```

---

## Demo Accounts

admin@manga.local        — Admin
mangaka@manga.local      — Mangaka
assistant@manga.local    — Assistant
editor@manga.local       — Editor (Tantou)
board@manga.local        — EditorialBoard

---

## Document Index

| File | Purpose |
|---|---|
| AGENTS.md | Global AI agent rules — read first |
| docs/MASTER_AGENT_PLAN.md | This file — overall phase plan |
| docs/AI_AGENT_EXECUTION_RULES.md | Hard rules for every coding agent |
| docs/PHASE_2_MVC_CRUD_PLAN.md | Detailed Phase 2 implementation guide |
| docs/PHASE_3_RAZOR_PAGES_WORKFLOW_PLAN.md | Detailed Phase 3 guide |
| docs/PHASE_4_BLAZOR_SIGNALR_PLAN.md | Detailed Phase 4 guide |
| docs/PHASE_5_WORKER_FINAL_DEMO_PLAN.md | Detailed Phase 5 guide |
| prompts/PHASE_2_IMPLEMENTATION_PROMPT.md | Ready-to-copy Phase 2 agent prompt |
| prompts/PHASE_3_IMPLEMENTATION_PROMPT.md | Ready-to-copy Phase 3 agent prompt |
| prompts/PHASE_4_IMPLEMENTATION_PROMPT.md | Ready-to-copy Phase 4 agent prompt |
| prompts/PHASE_5_IMPLEMENTATION_PROMPT.md | Ready-to-copy Phase 5 agent prompt |
| docs/PROJECT_STATUS.md | Live project status tracking |
