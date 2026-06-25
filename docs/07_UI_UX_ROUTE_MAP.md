# 07 — UI/UX Route Map

## Layout

Use a single Bootstrap-based layout with role-specific navigation.

Top navigation:

- Dashboard.
- Series.
- Tasks.
- Editor Review.
- Board.
- Ranking.
- Notifications.
- Admin.

Hide menu items by role.

## Suggested routes

### Public/auth routes

| Route | Type | Purpose |
|---|---|---|
| `/` | MVC | Landing page / redirect by role |
| `/Auth/Login` | MVC | Demo login |
| `/Auth/Logout` | MVC | Logout |

### Mangaka MVC routes

| Route | Type | Purpose |
|---|---|---|
| `/Series` | MVC | List series |
| `/Series/Details/{id}` | MVC | Series detail |
| `/Series/Create` | MVC | Create series |
| `/Series/Edit/{id}` | MVC | Edit series |
| `/Series/Submit/{id}` | MVC POST | Submit series |
| `/Chapters?seriesId={id}` | MVC | Chapters under series |
| `/MangaPages?chapterId={id}` | MVC | Pages under chapter |
| `/PageRegions/Create?pageId={id}` | MVC | Create region |
| `/Tasks/Create?pageId={id}` | MVC | Assign production task |
| `/Submissions/Review/{taskId}` | MVC | Review assistant submission |

### Assistant Razor Pages routes

| Route | Type | Purpose |
|---|---|---|
| `/Assistant/Tasks` | Razor Page | Task inbox |
| `/Assistant/Tasks/Details/{id}` | Razor Page | Task detail |
| `/Assistant/Tasks/Submit/{id}` | Razor Page | Submit work |
| `/Assistant/Earnings` | Razor Page | Monthly earnings |

### Editor routes

| Route | Type | Purpose |
|---|---|---|
| `/Editor/Series` | MVC | Assigned series |
| `/Editor/Pages/{pageId}` | MVC | Page review |
| `/EditorComments/Create?pageId={id}` | MVC | Add comment |
| `/EditorComments/Resolve/{id}` | MVC POST | Resolve comment |

### Board routes

| Route | Type | Purpose |
|---|---|---|
| `/Board/PendingSeries` | MVC | Series waiting for vote |
| `/Board/Vote/{seriesId}` | MVC | Vote screen |
| `/Board/Decision/{seriesId}` | MVC | Publishing decision |
| `/Ranking` | MVC | Ranking table |
| `/Ranking/Import` | MVC | Enter reader vote data |

### Dashboard routes

| Route | Type | Purpose |
|---|---|---|
| `/Dashboard` | Blazor/MVC host | Dashboard shell |
| `/dashboard/mangaka` | Blazor | Mangaka dashboard |
| `/dashboard/assistant` | Blazor | Assistant dashboard |
| `/dashboard/editor` | Blazor | Editor dashboard |
| `/dashboard/board` | Blazor | Board dashboard |

### Admin routes

| Route | Type | Purpose |
|---|---|---|
| `/Admin/Users` | MVC | User list |
| `/Admin/Notifications` | MVC | Notification list |
| `/Admin/BackgroundJobs` | MVC | Worker logs |
| `/Admin/AuditLogs` | MVC | Audit logs |
| `/Admin/AiJobs` | MVC | AI job list |

## Page design guidelines

### List pages

Every list page should support at least:

- Search keyword if relevant.
- Status filter if relevant.
- Pagination if list can grow.
- Action buttons according to role.

### Detail pages

Detail pages should show:

- Main entity data.
- Current status.
- Related records.
- Workflow history if available.
- Valid next actions.

### Forms

Every form should have:

- Validation summary.
- Required field markers.
- Cancel/back button.
- Clear success/error message.

### Dashboard cards

Minimum dashboard cards:

- Total series.
- Active chapters.
- Pending tasks.
- Overdue tasks.
- Latest ranking.
- Unread notifications.

## Demo data labels

Use demo series names:

- Shadow Ink.
- Neon Samurai.
- Crimson Café.
- Moon Rabbit Express.
- Hollow Brush.
- Starfall Idol Manga.
- Paper Dragon Legacy.

These names already exist in SQL seed scripts.
