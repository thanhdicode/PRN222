# 11 — Demo and Presentation Playbook

## Demo goal

Show that MangaWorkflow is not just CRUD. It applies all PRN222 major topics in a coherent workflow.

## Recommended demo length

8–12 minutes.

## Demo story

Use this story:

> A manga studio is producing new series. Mangaka submits a series, board votes, manga pages are uploaded, assistant tasks are assigned, assistant submits work, mangaka reviews, editor comments, ranking data is imported, notifications appear realtime, and background jobs monitor deadlines/risk.

## Demo accounts

```text
admin@manga.local
mangaka@manga.local
assistant@manga.local
editor@manga.local
board@manga.local
mangaka2@manga.local
assistant2@manga.local
assistant3@manga.local
editor2@manga.local
board2@manga.local
board3@manga.local
```

Password for every demo account: `test123@`.

## Demo flow

### Step 1 — Explain technology map

Say:

> This project uses MVC for management modules, Razor Pages for assistant workflows, Blazor for dashboards, SignalR for realtime notifications, Worker Service for background jobs, EF Core/SQL Server for persistence, DI for architecture, and async/await for database and notification operations.

### Step 2 — Admin verifies seed data

Show:

- Users.
- Roles.
- Background job logs.
- Notifications.

### Step 3 — Mangaka series/chapter/page flow

Login as Mangaka.

Show:

- Series list.
- `Crimson Café` as Draft.
- `Neon Samurai` as Submitted.
- Chapter list.
- Manga pages.
- Page regions.

### Step 4 — Task assignment

Create a task for assistant or show existing:

- Task type.
- Linked page region.
- Deadline.
- Assistant.
- Price.

Point out that this creates a notification.

### Step 5 — Assistant Razor Pages flow

Login/open second browser as Assistant.

Show:

- Assistant task inbox.
- Task detail.
- Submit work.
- Earnings page.

### Step 6 — SignalR live notification

In first browser, keep notification panel open. In second browser, submit task. Show live notification.

If live SignalR fails, show database notification record and explain the hub code.

### Step 7 — Mangaka review

Login as Mangaka.

Show:

- Pending submissions.
- Approve/reject/request revision.
- Review note.
- Assistant receives notification.

### Step 8 — Editor review

Login as Editor.

Show:

- Assigned series.
- Page comments.
- Create/resolve comment.

### Step 9 — Board voting and ranking

Login as Board.

Show:

- Pending series.
- Vote screen.
- Existing votes.
- Reader vote import.
- Ranking records.
- Cancellation risk.

### Step 10 — Blazor dashboard

Show dashboard cards:

- Chapter progress.
- Task status count.
- Ranking trends.
- Overdue tasks.
- Unread notifications.
- Background jobs.

### Step 11 — Worker Service

Show:

- Worker console or Admin job log screen.
- `OverdueTaskScannerJob`.
- `DeadlineReminderJob`.
- `RankingRiskScannerJob`.

### Step 12 — Optional AI module

Only demo if stable:

- Show `AiSegmentationJobs`.
- Queue mock segmentation.
- Insert suggested `PageRegions`.
- Explain no real AI training is required for PRN222.

## Presentation slide structure

1. Title and team.
2. Problem context.
3. User roles.
4. System workflow.
5. Database design.
6. PRN222 technology mapping.
7. Live demo.
8. Challenges and lessons learned.
9. Future improvements.

## Rubric argument

Use this table in presentation:

| PRN222 item | Where shown |
|---|---|
| MVC | Series/Board/Ranking/Admin modules |
| Razor Pages | Assistant task inbox/submission |
| Blazor | Dashboard |
| SignalR | Realtime notifications |
| Worker Service | Deadline/ranking jobs |
| EF Core/SQL Server | All persistent data |
| DI | Services and repositories |
| Async | EF queries and notifications |

## Backup plan

If something fails live:

- Have screenshots of SignalR notification.
- Have SQL queries ready.
- Have seed data ready.
- Have a branch/tag named `demo-stable`.
