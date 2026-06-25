# 12 — AI Agent Operating Manual

This file tells AI agents how to work on this repo without going out of scope.

## Read order for every new session

1. `AGENTS.md`
2. `docs/00_PROJECT_BRIEF.md`
3. `docs/02_PRN222_ALIGNMENT_MATRIX.md`
4. `docs/05_IMPLEMENTATION_PLAYBOOK.md`
5. `docs/PROJECT_STATUS.md`
6. Current issue/task prompt

## Standard agent workflow

For any coding task:

1. Identify which PRN222 topic the task demonstrates.
2. Identify affected files.
3. Inspect existing code before editing.
4. Make the smallest coherent change.
5. Build the solution.
6. Run relevant manual/automated tests.
7. Update `docs/PROJECT_STATUS.md`.

## Good task prompt format

```text
Task: Implement Assistant task inbox using Razor Pages.
Course evidence: Razor Pages, EF Core async, DI, repository/service.
Scope: Pages/Assistant/Tasks/Index only.
Use existing database tables: ProductionTasks, TaskStatuses, MangaPages, PageRegions, Users.
Do not implement: authentication redesign, Blazor dashboard, SignalR yet.
Acceptance: assistant can see seeded tasks filtered by status.
```

## Bad prompt examples

Avoid prompts like:

```text
Build the whole project.
Make it enterprise-grade.
Add all features.
Use the best architecture.
Make it production-ready.
```

These cause overengineering.

## Agent roles

### Architect agent

Responsible for:

- Ensuring scope stays PRN222-aligned.
- Preventing overengineering.
- Maintaining solution structure.

### Backend agent

Responsible for:

- EF Core queries.
- Repositories.
- Services.
- Business rules.

### MVC agent

Responsible for:

- Controllers.
- Views.
- ViewModels.
- Validation.

### Razor Pages agent

Responsible for:

- Assistant workflows.
- PageModel handlers.
- Upload/submit forms.

### Blazor agent

Responsible for:

- Dashboard components only.

### Realtime/Worker agent

Responsible for:

- SignalR hubs.
- Client connection scripts.
- Worker services.

### QA agent

Responsible for:

- Manual test checklist.
- Build errors.
- Demo flow.

## Required output after each agent session

Every coding agent must end with:

```text
Changed files:
- ...

Build/test result:
- ...

PRN222 evidence covered:
- ...

Known issues:
- ...

Next step:
- ...
```

## Prompt safety for this project

Do not ask AI to:

- invent new database schema without checking SQL files.
- replace the whole architecture.
- remove seed data.
- implement real payment.
- implement real AI model training.
- migrate to microservices.
- switch to a JS SPA framework.

## Recommended AI tools for this repo

- Codex or Claude Code for large multi-file changes.
- Cline/Roo Code for IDE-based implementation.
- Aider for small git-tracked terminal changes.
- GitHub Copilot for inline code and review.
- Continue/Cursor rules for consistent project instructions.
