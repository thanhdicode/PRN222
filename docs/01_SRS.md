# 01 — Software Requirements Specification (SRS)

## 1. Introduction

### 1.1 Purpose

This SRS defines the requirements for the MangaWorkflow web application. It is written for students, AI coding agents, instructors, and reviewers who need a precise understanding of what to build.

### 1.2 Scope

The system manages the manga creation and publishing workflow from series proposal to ranking and publishing decision. It includes role-based modules for Mangaka, Assistant, Tantou Editor, Editorial Board, and Admin.

### 1.3 Technology constraints

- ASP.NET Core .NET 8 or later.
- C#.
- SQL Server 2019 or later.
- EF Core.
- MVC.
- Razor Pages.
- Blazor.
- SignalR.
- Worker Service or Hosted Service.
- Visual Studio 2022 or later.

## 2. Overall description

### 2.1 Product perspective

The system is a full-stack web application. It uses a SQL Server relational database as the source of truth. Application logic is implemented in services and repositories. UI is split intentionally:

- MVC for management modules.
- Razor Pages for assistant workflow pages.
- Blazor for dashboard.
- SignalR for realtime updates.
- Worker Service for background scanning.

### 2.2 User classes

| User class | Description |
|---|---|
| Admin | Manages users, lookups, logs, and system data. |
| Mangaka | Creates series/chapters/pages and assigns/reviews work. |
| Assistant | Works on assigned tasks and uploads submissions. |
| Tantou Editor | Reviews content and page quality. |
| Editorial Board | Votes, decides publishing, imports ranking data. |

### 2.3 Assumptions

- This is an academic project, not production software.
- File uploads may be stored under `wwwroot/uploads` or represented by demo URLs.
- User authentication may start with a simple custom login using seeded accounts.
- Optional AI segmentation can be mocked; no real model training is required.
- Email sending can be mocked by Notifications table.

### 2.4 Constraints

- Must not overbuild beyond PRN222 requirements.
- Must use the provided SQL scripts as the baseline database.
- Must keep modules demoable with seed data.
- Must avoid external paid services.

## 3. Functional requirements

### FR-A — Authentication and authorization

| ID | Requirement | Priority | PRN222 evidence |
|---|---|---|---|
| FR-A1 | User can login and logout. | Must | Web app basics, security. |
| FR-A2 | User has one or more roles: Admin, Mangaka, Assistant, TantouEditor, EditorialBoard. | Must | Role-based UI. |
| FR-A3 | System shows role-specific menus. | Must | MVC/Razor layout. |
| FR-A4 | Unauthorized users cannot access role-only pages. | Must | Authorization. |
| FR-A5 | Admin can list users and roles. | Should | MVC CRUD/list. |

### FR-B — Series management

| ID | Requirement | Priority |
|---|---|---|
| FR-B1 | Mangaka can view own series. | Must |
| FR-B2 | Mangaka can create series with title, description, genre, cover URL, editor. | Must |
| FR-B3 | Mangaka can edit series while Draft or RevisionRequired. | Must |
| FR-B4 | Mangaka can submit series to board. | Must |
| FR-B5 | System records series status history. | Should |
| FR-B6 | Editor/Board can filter series by status. | Must |
| FR-B7 | Board can see series proposals waiting for review. | Must |

### FR-C — Chapter and manuscript management

| ID | Requirement | Priority |
|---|---|---|
| FR-C1 | Mangaka can create chapter under a series. | Must |
| FR-C2 | Mangaka can set chapter deadline. | Must |
| FR-C3 | Mangaka can upload or register manuscript file URL. | Must |
| FR-C4 | System tracks manuscript status and version. | Should |
| FR-C5 | System lists chapters by status and deadline. | Must |
| FR-C6 | Chapter progress is calculated from task status. | Must |

### FR-D — Manga page and region management

| ID | Requirement | Priority |
|---|---|---|
| FR-D1 | Mangaka can upload/register manga pages. | Must |
| FR-D2 | Mangaka can view pages in a chapter. | Must |
| FR-D3 | Mangaka can create page regions with X, Y, Width, Height. | Must |
| FR-D4 | Region type can be Panel, SpeechBubble, Character, Background, Shading, Effect, Other. | Must |
| FR-D5 | Region can be marked Manual or AI. | Should |
| FR-D6 | Optional AI job can create suggested regions. | Optional |

### FR-E — Production task assignment

| ID | Requirement | Priority |
|---|---|---|
| FR-E1 | Mangaka can assign a task to an assistant. | Must |
| FR-E2 | Task must have title, description, type, priority, deadline, status, price. | Must |
| FR-E3 | Task may be linked to a page region. | Must |
| FR-E4 | Assistant receives notification when task is assigned. | Must |
| FR-E5 | Task statuses include Assigned, InProgress, Submitted, RevisionRequired, Approved, Rejected, Overdue, Cancelled. | Must |

### FR-F — Assistant workflow

| ID | Requirement | Priority |
|---|---|---|
| FR-F1 | Assistant can view own task inbox. | Must |
| FR-F2 | Assistant can filter tasks by status. | Must |
| FR-F3 | Assistant can mark assigned task as InProgress. | Should |
| FR-F4 | Assistant can upload/register submission file. | Must |
| FR-F5 | Assistant can add submission comment. | Must |
| FR-F6 | Assistant can see approved tasks and earnings. | Should |

### FR-G — Mangaka review

| ID | Requirement | Priority |
|---|---|---|
| FR-G1 | Mangaka can view submitted tasks for own series/pages. | Must |
| FR-G2 | Mangaka can approve submission. | Must |
| FR-G3 | Mangaka can reject submission. | Must |
| FR-G4 | Mangaka can request revision with review note. | Must |
| FR-G5 | System updates task status based on review decision. | Must |
| FR-G6 | Assistant receives SignalR notification when reviewed. | Must |

### FR-H — Editor comments

| ID | Requirement | Priority |
|---|---|---|
| FR-H1 | Tantou Editor can view assigned series and pages. | Must |
| FR-H2 | Editor can create page comment with optional bounding box. | Must |
| FR-H3 | Editor can mark comment as resolved. | Must |
| FR-H4 | Mangaka receives notification when editor comments. | Must |
| FR-H5 | Editor can see unresolved comments by chapter. | Should |

### FR-I — Editorial board voting

| ID | Requirement | Priority |
|---|---|---|
| FR-I1 | Board member can see submitted/under-review series. | Must |
| FR-I2 | Board member can vote Approve, Reject, NeedRevision, Abstain. | Must |
| FR-I3 | One board member can vote once per series. | Must |
| FR-I4 | Board can add comment to vote. | Must |
| FR-I5 | System can display vote summary. | Must |
| FR-I6 | Board can create publishing decision: Continue, Cancel, ChangeSchedule, Pause, Promote. | Should |

### FR-J — Reader vote and ranking

| ID | Requirement | Priority |
|---|---|---|
| FR-J1 | Board can import/enter reader vote data per issue. | Must |
| FR-J2 | System stores issue number, vote count, rank position. | Must |
| FR-J3 | System stores ranking records with trend Up, Down, Stable, New. | Must |
| FR-J4 | System shows latest ranking dashboard. | Must |
| FR-J5 | System flags series with high cancellation risk. | Should |

### FR-K — Notifications and SignalR

| ID | Requirement | Priority |
|---|---|---|
| FR-K1 | System stores notifications in database. | Must |
| FR-K2 | System pushes task assignment notification realtime. | Must |
| FR-K3 | System pushes submission uploaded notification realtime. | Must |
| FR-K4 | System pushes review result notification realtime. | Must |
| FR-K5 | System pushes editor comment/ranking/deadline notifications. | Should |
| FR-K6 | User can mark notification as read. | Should |

### FR-L — Blazor dashboard

| ID | Requirement | Priority |
|---|---|---|
| FR-L1 | Dashboard shows chapter progress. | Must |
| FR-L2 | Dashboard shows task status counts. | Must |
| FR-L3 | Dashboard shows latest ranking. | Must |
| FR-L4 | Dashboard shows unread notifications. | Must |
| FR-L5 | Dashboard updates after action or refresh. | Must |
| FR-L6 | Dashboard can use SignalR for live updates. | Should |

### FR-M — Worker Service / Hosted Service

| ID | Requirement | Priority |
|---|---|---|
| FR-M1 | Worker scans tasks near deadline and creates warning notifications. | Must |
| FR-M2 | Worker marks overdue tasks. | Must |
| FR-M3 | Worker calculates ranking risk from ranking data. | Should |
| FR-M4 | Worker cleans old notifications or logs skipped cleanup. | Should |
| FR-M5 | Worker writes execution result to BackgroundJobLogs. | Must |

### FR-N — Optional AI module

| ID | Requirement | Priority |
|---|---|---|
| FR-N1 | Mangaka can request AI segmentation job for a page. | Optional |
| FR-N2 | System stores job status: Queued, Running, Succeeded, Failed. | Optional |
| FR-N3 | System stores raw JSON result. | Optional |
| FR-N4 | Successful job can create PageRegions with SourceType = AI. | Optional |

## 4. Non-functional requirements

### NFR-1 Maintainability

Use a clear layered structure. UI must not contain business rules.

### NFR-2 Performance

Use pagination for large lists. Use async queries. Use indexed columns already present in SQL scripts.

### NFR-3 Security

Validate user inputs. Validate uploads. Prevent unauthorized access by role and ownership.

### NFR-4 Reliability

Workflow status changes must be consistent. Do not approve a task without a submitted submission. Do not mark a series decision without a valid user.

### NFR-5 Usability

Every role should land on a dashboard relevant to that role. Do not show irrelevant actions.

### NFR-6 Demo-readiness

All major screens must have seed data from SQL v2/v3.

## 5. Acceptance criteria by role

### Mangaka acceptance criteria

- Can create/edit/submit series.
- Can create chapters/pages.
- Can assign page-region task.
- Can review assistant submissions.
- Can see ranking and warnings.

### Assistant acceptance criteria

- Can see own tasks.
- Can submit work.
- Can see review status and earnings.

### Editor acceptance criteria

- Can see assigned series/pages.
- Can comment and resolve comments.
- Can see progress and deadline risk.

### Board acceptance criteria

- Can vote on series.
- Can create decisions.
- Can import reader votes.
- Can see ranking dashboard.

### Admin acceptance criteria

- Can view users.
- Can view notifications/logs/audit trail.
- Can verify background job results.
