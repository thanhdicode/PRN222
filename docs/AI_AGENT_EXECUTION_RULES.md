# AI AGENT EXECUTION RULES
# PRN222 MangaWorkflow System — Hard Rules for Every Coding Agent

**IMPORTANT**: Every AI coding agent MUST read this file before making any code change.
These rules are non-negotiable. Violating them will cause broken builds and failed PRN222 demonstrations.

---

## Rule 1: Read Before You Code

Before writing any code, read these files in order:
1. AGENTS.md
2. docs/MASTER_AGENT_PLAN.md (this file references it)
3. docs/AI_AGENT_EXECUTION_RULES.md (this file)
4. The specific phase plan file for the phase you are working on
5. docs/PROJECT_STATUS.md to confirm what is already done

If any of these files are missing, stop and report the missing file instead of guessing.

---

## Rule 2: Do Not Jump Phases

Phase order is mandatory:
- Phase 1 DONE — do not redo it
- Phase 2 is next — implement Phase 2 only when instructed
- Phase 3 may not start until Phase 2 is DONE
- Phase 4 may not start until Phase 3 is DONE
- Phase 5 may not start until Phase 4 is DONE

If you are instructed to implement a later phase but an earlier phase is not complete,
refuse and explain which earlier phase must be finished first.

---

## Rule 3: Build Must Pass Before and After

Before making any change, verify the build passes:
```
dotnet clean
dotnet restore
dotnet build
```

After making changes, run the same commands. If the build fails after your changes,
fix the build before moving on. Do not leave broken builds.

Also run DbSmokeTest after every major change:
```
dotnet run --project MangaWorkflow.Tools.DbSmokeTest
```

---

## Rule 4: Architecture Dependency Direction is Sacred

Never violate these rules:

| Layer | May reference | May NOT reference |
|---|---|---|
| Domain | Nothing | Everything |
| Application | Domain | Infrastructure, Web, Worker |
| Infrastructure | Domain, Application | Web, Worker |
| Web | Domain, Application, Infrastructure | Worker |
| Worker | Domain, Application, Infrastructure | Web |
| Tools.DbSmokeTest | Domain, Infrastructure | Application, Web, Worker |

If you need something from Infrastructure inside Application, create an interface in
Application and let Infrastructure implement it.

---

## Rule 5: No Direct DbContext in Controllers or PageModels

Controllers call Services only.
Services call Repositories.
Repositories access MangaWorkflowDbContext.

FORBIDDEN pattern:
```csharp
// In a Controller — WRONG
var series = await _context.Series.ToListAsync(); // DO NOT DO THIS
```

CORRECT pattern:
```csharp
// In a Controller — CORRECT
var series = await _seriesService.GetAllAsync(cancellationToken);
```

---

## Rule 6: All Database Calls Must Be Async

Every EF Core method must use its async variant:
- ToListAsync() not ToList()
- FindAsync() not Find()
- FirstOrDefaultAsync() not FirstOrDefault()
- SaveChangesAsync() not SaveChanges()
- AnyAsync() not Any()
- CountAsync() not Count()

Never use .Result or .Wait() in request-handling code.

---

## Rule 7: Do Not Hardcode Status IDs

Status tables exist in the database (TaskStatuses, SeriesStatuses, etc.).
Do not hardcode integer IDs like status = 3.
Query status IDs by StatusCode string, or use constants.

Use the existing constants pattern in MangaWorkflow.Domain/Constants/ or create new ones.

Example:
```csharp
// CORRECT
var draftStatus = await _context.SeriesStatuses
    .FirstAsync(s => s.StatusCode == "Draft");
series.StatusId = draftStatus.StatusId;

// Or using a constant:
// public const string Draft = "Draft";
```

---

## Rule 8: Use DTOs for Data Transfer, Not Raw Entities

Controllers should pass DTOs or ViewModels to Views, not raw EF Core entity objects.
This keeps the presentation layer decoupled from the data layer.

Simple ViewModels are acceptable and preferred for PRN222.
Do not use AutoMapper unless the team explicitly approves it.

---

## Rule 9: Registration in DependencyInjection.cs

Every new service must be registered in:
- MangaWorkflow.Application/DependencyInjection.cs (for Application services)
- MangaWorkflow.Infrastructure/DependencyInjection.cs (for Infrastructure repositories)

Do not add ad-hoc registrations directly to Program.cs unless the team approves.

---

## Rule 10: Role Authorization is Required

Every controller and Razor Page must have appropriate role authorization:
- Admin area: [Authorize(Roles = "Admin")]
- Mangaka area: [Authorize(Roles = "Mangaka,Admin")]
- Board area: [Authorize(Roles = "EditorialBoard,Admin")]
- Assistant pages: [Authorize(Roles = "Assistant")]
- Editor pages: [Authorize(Roles = "Editor,Admin")]

Use the exact role names from the Roles table seed data in the database.

---

## Rule 11: UI Must Be Simple Bootstrap

Use Bootstrap 5 (already included in the MVC template).
Do not add React, Vue, Angular, Tailwind, or complex CSS frameworks.
JavaScript is allowed only for:
- SignalR client connection (Phase 4)
- Simple UX helpers (e.g., confirm delete dialog)
- Basic form interactivity

---

## Rule 12: File Upload Safety Rules

When implementing file uploads (Phase 2 pages, Phase 3 submissions):
- Validate file extension: only allow .jpg, .jpeg, .png, .webp for images; .zip, .psd for submissions
- Validate content type
- Validate file size (suggest 10 MB max for demo)
- Save files to wwwroot/uploads/ with a GUID filename to prevent collisions
- Never execute uploaded files

---

## Rule 13: Update Documentation After Every Session

At the end of every coding session, update:
- docs/PROJECT_STATUS.md with what was done and what is next
- Any TODOs found during implementation
- Exact commands run and their results
- Any known errors or limitations

If completing a phase, create:
- docs/PHASE_X_COMPLETION_REPORT.md

---

## Rule 14: Do Not Delete Existing Documents

Do not delete any existing .md files in docs/ or prompts/.
Do not delete seed data.
Do not restructure the existing entity files in Domain/Entities/ without explicit approval.
If you must change something structural, document why in PROJECT_STATUS.md.

---

## Rule 15: Banned Technologies (Absolute)

Under no circumstances add these to the project:
- CQRS or MediatR packages
- Microservices or service mesh
- Docker configuration
- Redis or distributed cache
- RabbitMQ or message brokers
- React, Angular, Vue, or Next.js
- GraphQL
- Real AI/ML model training code
- Payment system integrations
- Production-grade OAuth server
- Unit-of-Work abstraction on top of EF Core

---

## Rule 16: Phase 2 Specific Rules

If implementing Phase 2 (MVC CRUD):
- Use Areas: Areas/Admin, Areas/Mangaka, Areas/Board
- Each Area needs an _ViewImports.cshtml and _ViewStart.cshtml
- Register Area routes in Program.cs with MapAreaControllerRoute
- Do NOT build Razor Pages workflow here
- Do NOT add SignalR client JavaScript here
- Do NOT create Blazor components here
- Notification records may be INSERTED in the database (for Board votes, etc.)
  but do NOT push SignalR events yet

---

## Rule 17: Phase 3 Specific Rules

If implementing Phase 3 (Razor Pages):
- All workflow pages go under Pages/ folder (not Areas)
- Use OnGetAsync and OnPostAsync handler naming correctly
- Use TempData for flash messages after redirects
- Use [BindProperty] for form fields
- Notification records are inserted in the database but no SignalR push yet
- Do NOT create Blazor components here

---

## Rule 18: Phase 4 Specific Rules

If implementing Phase 4 (Blazor + SignalR):
- Blazor dashboards go in Components/Dashboard/ or Components/Pages/
- Do NOT rewrite MVC or Razor Pages in Blazor
- SignalR hub MapHub calls go in Program.cs
- Start SignalR with Clients.All before attempting user groups
- SignalR JavaScript client goes in wwwroot/js/notifications.js
- Include signalr.js via CDN or local lib (already in libman)

---

## Rule 19: Phase 5 Specific Rules

If implementing Phase 5 (Workers):
- Workers go in MangaWorkflow.Worker/
- Each Worker creates its own IServiceScope using IServiceScopeFactory
- Workers write to BackgroundJobLogs table
- Workers use CancellationToken for clean shutdown
- Workers run a delay loop: await Task.Delay(interval, stoppingToken)

---

## Quick Sanity Checklist Before Submitting Code

[ ] dotnet build passes with 0 errors
[ ] dotnet run --project MangaWorkflow.Tools.DbSmokeTest passes
[ ] No direct DbContext in controllers or pages
[ ] All DB calls use async methods
[ ] New services/repos registered in DependencyInjection.cs
[ ] No banned technologies added
[ ] Authorization attributes are on all new controllers/pages
[ ] docs/PROJECT_STATUS.md updated
