# MangaWorkflow AI Demo Script

This script walks through demonstrating the AI features to a client or team member.

## 1. Preparation
1. Run the Database Smoke Test to ensure the DB is migrated and seeded.
   ```bash
   dotnet run --project MangaWorkflow.Tools.DbSmokeTest
   ```
2. Start the AI Python API locally.
   ```bash
   cd MangaWorkflow.AI
   uvicorn src.api.main:app --reload --port 8001
   ```
3. Run the .NET Web Application.
   ```bash
   cd MangaWorkflow.Web
   dotnet run
   ```

## 2. Demonstration Steps

### Step 1: Login
- Log in to the application as a **Mangaka** (e.g., `mangaka1@test.com`).
- Navigate to the **AI Studio** tab in the top navigation bar.

### Step 2: Trigger Segmentation
- Explain to the client: *"This represents our Manga Page analysis engine powered by YOLOv8."*
- Enter a valid Page ID (or use any dummy GUID, the system currently mocks the result if using the mock client).
- Click **Run AI Segmentation**.

### Step 3: Review Segmentation Results
- Show the visual overlay.
- Point out the detected regions (Panels, Text, Background) and their AI confidence scores.
- Mention: *"The AI seamlessly integrates Python-based computer vision into the .NET ecosystem."*

### Step 4: Suggest Tasks
- Click **Generate Task Suggestions**.
- Show the auto-generated tasks based on regions (e.g., *Typeset Dialogue* for Text regions, *Draw Background* for Background regions).
- Highlight the **Complexity Score** and state: *"The AI suggests tasks, but keeps the human in the loop by requiring the Mangaka to approve them before assigning to an Assistant."*

## 3. Code Walkthrough Highlights
- Show `MangaWorkflow.AI/src/api/main.py` to highlight the Python FastAPI structure.
- Show `AiStudioService.cs` in `MangaWorkflow.Infrastructure` to explain how .NET communicates with Python and saves results to the DB.
- Show `MangaWorkflowDB_v4_ai_extension.sql` to demonstrate the clean migration path that doesn't break the existing v1-v3 schema.
