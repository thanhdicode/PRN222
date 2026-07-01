# Final Project Demo Script

## 1. Setup & Starting Services
1. Open terminal and run the database script: `db/MangaWorkflowDB_v2_demo_ready.sql` to populate initial data.
2. Open Solution in Visual Studio or CLI.
3. Set both `MangaWorkflow.Web` and `MangaWorkflow.Worker` as Startup Projects.
4. Run the solution. Both Web UI and Worker console should start.

## 2. Authentication & Authorization (Phase 2)
1. **Login as Admin**: `admin@manga.local` / `test123@`
   - Show User Management page.
   - Show Role-based hiding of features.

## 3. Workflow Engine & CRUD (Phase 2 & 3)
1. **Login as Mangaka**: `mangaka@manga.local` / `test123@`
   - Navigate to "My Series" -> View *One Piece*.
   - Add a new Chapter.
   - Inside Chapter, add a Page and assign regions to Assistants.
2. **Login as Assistant**: `assistant@manga.local` / `test123@`
   - Navigate to "My Tasks".
   - Submit work for a specific region.
3. **Login as Mangaka**:
   - Go to "Review Submissions".
   - Approve or Reject the Assistant's work.

## 4. Blazor & SignalR (Phase 4)
1. **Open two browser windows**:
   - Window 1: Mangaka (`mangaka@manga.local` / `test123@`)
   - Window 2: Assistant (`assistant@manga.local` / `test123@`)
2. **SignalR Test**:
   - As Mangaka, reject a submission.
   - In Window 2 (Assistant), notice the notification bell badge increment and the toast popup *in real-time*.
3. **Blazor Dashboard Test**:
   - Go to Mangaka Dashboard.
   - Show the dynamic charts and lists updating via Blazor Server components without a full page refresh.

## 5. Worker Service & Background Jobs (Phase 5)
1. **Login as Admin** (`admin@manga.local` / `test123@`) and navigate to `Admin -> Background Jobs`.
2. **Run Deadline Reminders**: Click "Run Now". See the success message.
3. **Run Overdue Task Scanner**: Click "Run Now". The worker will mark any expired uncompleted tasks as "Overdue".
4. **Log verification**:
   - Query `BackgroundJobLogs` table in SQL Server Management Studio to prove the jobs successfully ran, recorded start time, end time, and output message.
5. Demonstrate the console logs output from `MangaWorkflow.Worker`.

## 6. Closing
- The PRN222 Syllabus requirements are completely fulfilled. Clean Architecture, EF Core, Identity, MVC, Razor Pages, Blazor, SignalR, and Worker Services all function together harmoniously.
