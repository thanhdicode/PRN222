# 06 — Feature Backlog and User Stories

## Priority system

- P0: required for final demo.
- P1: strongly recommended.
- P2: optional polish.
- P3: out of scope unless everything is done.

## Epic 1 — Authentication and role navigation

### US-1.1 Login as demo user

As a user, I want to login using a seeded email so that I can access role-specific screens.

Acceptance:

- Login page exists.
- User can choose or type email.
- Role claims/session are created.
- Layout changes by role.

Priority: P0

### US-1.2 Role-based menu

As a user, I want to see only actions relevant to my role.

Priority: P0

## Epic 2 — Mangaka series and chapter workflow

### US-2.1 Create series

As a Mangaka, I want to create a new series proposal.

Fields:

- Title.
- Alternative title.
- Description.
- Genre.
- Cover image URL.
- Tantou editor.
- Publication schedule.

Priority: P0

### US-2.2 Submit series

As a Mangaka, I want to submit a Draft series for board review.

Acceptance:

- Status changes from Draft to Submitted.
- WorkflowStatusHistories row is created.
- Notification can be created for board role.

Priority: P0

### US-2.3 Manage chapters

As a Mangaka, I want to create and view chapters under a series.

Priority: P0

### US-2.4 Upload/register manga pages

As a Mangaka, I want to add pages to a chapter so that I can assign work.

Priority: P0

## Epic 3 — Page region and task assignment

### US-3.1 Create page region

As a Mangaka, I want to define a rectangular region on a page.

Priority: P0

### US-3.2 Assign task to assistant

As a Mangaka, I want to assign a task linked to a page region.

Acceptance:

- Task appears in assistant inbox.
- Notification is created.
- SignalR event is sent if connected.

Priority: P0

## Epic 4 — Assistant workflow

### US-4.1 View task inbox

As an Assistant, I want to see tasks assigned to me.

Priority: P0

### US-4.2 Submit completed work

As an Assistant, I want to upload or register a file URL for a task.

Priority: P0

### US-4.3 Track earnings

As an Assistant, I want to see approved task earnings.

Priority: P1

## Epic 5 — Mangaka review

### US-5.1 Review submitted task

As a Mangaka, I want to approve, reject, or request revision on assistant submissions.

Priority: P0

### US-5.2 Notify assistant of result

As a system, I want to notify the assistant when the submission is reviewed.

Priority: P0

## Epic 6 — Editor review

### US-6.1 Add editor comment

As a Tantou Editor, I want to comment on a manga page.

Priority: P0

### US-6.2 Resolve editor comment

As a Mangaka or Editor, I want to mark comments as resolved.

Priority: P1

## Epic 7 — Board voting and publishing

### US-7.1 Vote on series

As an Editorial Board member, I want to vote on submitted series.

Priority: P0

### US-7.2 Create publishing decision

As a Board member, I want to decide continue/cancel/change schedule/pause/promote.

Priority: P1

## Epic 8 — Ranking

### US-8.1 Enter reader voting data

As a Board member, I want to enter reader vote data after issue release.

Priority: P0

### US-8.2 View latest ranking

As a Board member or Mangaka, I want to view rankings and trends.

Priority: P0

## Epic 9 — Realtime notifications

### US-9.1 Receive notification in realtime

As a user, I want to receive live notifications when important workflow actions happen.

Priority: P0

Events:

- TaskAssigned.
- SubmissionUploaded.
- SubmissionReviewed.
- EditorCommentCreated.
- RankingUpdated.
- DeadlineWarning.

## Epic 10 — Blazor dashboard

### US-10.1 View dashboard

As each role, I want a dashboard summarizing work status.

Priority: P0

Widgets:

- Task counts.
- Chapter progress.
- Ranking.
- Notifications.
- Worker logs.

## Epic 11 — Worker Service

### US-11.1 Mark overdue tasks

As a system, I want to mark tasks overdue when deadline passes.

Priority: P0

### US-11.2 Send deadline warning

As a system, I want to create deadline warnings for tasks due soon.

Priority: P0

### US-11.3 Scan ranking risk

As a system, I want to warn when ranking drops.

Priority: P1

## Epic 12 — Optional AI

### US-12.1 Mock AI segmentation

As a Mangaka, I want the system to suggest page regions automatically.

Priority: P2

Do not implement real ML training.
