# 00 — Project Brief

## Project name

**Manga Creation Workflow and Publishing Management System**

Short name: **MangaWorkflow**

## Academic context

This is a PRN222 big assignment. The project exists to demonstrate advanced cross-platform web application programming with .NET. It must not drift into a research-only project or a huge commercial platform.

## Problem statement

Manga production requires coordination between mangaka, assistants, tantou editors, and editorial boards. In a manual workflow, teams often use separate tools for file sharing, comments, task tracking, voting, ranking, and deadline monitoring. This causes confusion, duplicated work, unclear task ownership, and poor deadline visibility.

MangaWorkflow centralizes the production workflow into a role-based web application.

## Target users

### Mangaka

The manga author. Creates series, uploads manuscripts/pages, divides work by page region, assigns tasks, reviews assistant submissions, tracks ranking and cancellation risk.

### Assistant

Production helper. Receives assigned tasks, downloads page/resource context, submits work, tracks approved work and monthly earnings.

### Tantou Editor

Editor responsible for quality and schedule. Reviews manuscripts/pages, comments on visual/content issues, tracks progress and deadline risk.

### Editorial Board

Decision-making group. Votes on new series, decides publishing schedule, imports reader voting results, sees rankings, decides continue/cancel/change schedule.

### Admin

System owner. Manages users, roles, lookups, audit logs, and background job logs.

## Product vision

A focused manga studio workflow system that is complex enough to cover PRN222 but simple enough for a student team to finish.

## One demo scenario

The demo should follow **Shadow Ink**, **Neon Samurai**, or **Starfall Idol Manga**:

1. Mangaka submits a series.
2. Board votes.
3. Mangaka creates a chapter and uploads pages.
4. Mangaka assigns a page-region task to an assistant.
5. Assistant submits work.
6. Mangaka reviews the submission.
7. Editor adds a comment.
8. Board imports reader votes and sees ranking.
9. SignalR sends notifications.
10. Worker Service logs deadline/ranking checks.
11. Blazor dashboard shows the overall status.

## What the project is

- A web-based workflow management system.
- A PRN222 learning showcase.
- A database-backed ASP.NET Core application.
- A system with realtime collaboration-like updates.
- A demo-ready assignment with seeded data.

## What the project is not

- Not a manga reading site.
- Not a manga drawing tool.
- Not an online publishing platform for readers.
- Not a marketplace.
- Not a payment system.
- Not a real AI training platform.
- Not a microservice/cloud-native enterprise system.

## MVP success criteria

The project is successful when a teacher can see these PRN222 topics in the running app:

- MVC CRUD.
- Razor Pages task flow.
- Blazor dashboard.
- SignalR notification.
- Worker Service background processing.
- SQL Server + EF Core.
- Async/await.
- Dependency Injection.
- Layered architecture.
