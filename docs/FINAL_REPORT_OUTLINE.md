# MangaWorkflow System - Final Report

## 1. Executive Summary
The MangaWorkflow System is an enterprise-level web application designed to streamline the manga production pipeline for publishers, authors (Mangaka), editors, and assistants. Developed following the strict guidelines of the PRN222 curriculum, the application implements robust security, role-based workflows, real-time communications, and background processing.

## 2. Architecture & Technologies Used
- **Architecture**: Clean Architecture (Domain -> Application -> Infrastructure -> Web).
- **Database**: SQL Server accessed via Entity Framework Core.
- **Web UI**: ASP.NET Core MVC, Razor Pages, and Blazor Server components.
- **Authentication**: ASP.NET Core Identity with role-based policies.
- **Real-Time**: SignalR for immediate UI updates.
- **Background Jobs**: Worker Service utilizing `PeriodicTimer` and scoped dependency injection.

## 3. Key Implementations

### Phase 1 & 2: Data & Core Operations
Established the SQL Server database schema mapping and seeded default roles and users. Admin interfaces were built using MVC controllers and views to manage system users and overarching metadata.

### Phase 3: The Production Workflow (Razor Pages)
Implemented the core business logic of the manga production line. Razor pages were utilized to handle granular forms, such as assigning rectangular regions on a comic page to specific assistants, and allowing those assistants to submit completed drawing layers for review.

### Phase 4: Real-time Dashboards (Blazor + SignalR)
Blazor components were integrated to create rich, interactive dashboards for the Mangaka. SignalR Hubs were implemented and abstracted behind `INotificationService` to broadcast critical state changes (e.g., Task Approved, Task Overdue) to connected clients instantly.

### Phase 5: Automation (Worker Service)
A standalone Worker Service project was introduced. Five specialized hosted services run continuously in the background on periodic timers, monitoring the database for upcoming deadlines, expired tasks, unread notifications, and ranking risks, then utilizing the application layer to enforce business rules.

## 4. Challenges & Resolutions
- **Challenge**: Managing database context scopes inside long-running Worker tasks.
- **Resolution**: Utilized `IServiceScopeFactory` to generate fresh, short-lived scopes per tick, preventing DBContext threading issues and memory leaks.
- **Challenge**: SignalR dependency breaking the Clean Architecture rules (Application layer depending on Web).
- **Resolution**: Created a Notification interface in the Application layer, with a SignalR-specific implementation registered in the Web layer dependency injection container.

## 5. Conclusion
The MangaWorkflow System successfully validates the complete .NET Core curriculum (PRN222), proving mastery over modern backend, frontend, and daemon methodologies within the Microsoft technology stack.
