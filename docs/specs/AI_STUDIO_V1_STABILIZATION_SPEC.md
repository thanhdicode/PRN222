# AI Studio V1 Stabilization Specification

**Feature Name:** ai-studio-v1-stabilization  
**Branch:** feature/ai-studio-fullstack  
**Type:** Bugfix & Stabilization  
**Status:** In Progress  

---

## Executive Summary

This specification defines the stabilization and completion work required to make the MangaWorkflow AI Studio Extension demo-ready as v1. The AI Studio currently has incomplete implementation and architectural issues that prevent it from functioning as a cohesive, end-to-end AI-assisted workflow tool. This work focuses on fixing critical bugs, completing missing workflows, and ensuring architectural compliance with the established PRN222 layered architecture.

**Primary Goals:**
1. Complete the human-in-the-loop AI workflow (detect → review → accept → suggest → approve → execute)
2. Fix architectural violations (move business logic to Application layer, remove direct DbContext usage from services)
3. Enable real image data flow from .NET → Python API → .NET UI
4. Add missing controller actions and UI forms for accept/reject workflows
5. Ensure database integrity with proper constraints
6. Maintain stability of existing PRN222 Phase 0-5 features

**Non-Goals:**
- Real AI model training (mock mode is acceptable)
- New AI features beyond the planned workflow
- Rewriting the entire application
- Removing the Python API

---

## Requirements

### Functional Requirements

#### FR-1: Real Image Data Flow
**Priority:** Critical  
**Description:** AI Studio must use actual MangaPage image data throughout the workflow, not placeholder images.


**Acceptance Criteria:**
- MangaPage entity is loaded from repository with its ImageUrl/FileUrl
- Image path/URL is passed to Python API via AiSegmentationRequestDto
- Python API receives and uses the image data
- SegmentationResult.cshtml renders the actual manga page image
- Image dimensions are used for accurate overlay rendering

#### FR-2: AI Detection Persistence
**Priority:** Critical  
**Description:** AI detection results must be saved as AiDetectedRegions with proper relationships.

**Acceptance Criteria:**
- AiInferenceRequest record is created for each analysis
- DetectedRegions are saved with correct InferenceRequestId and PageId
- Confidence scores, coordinates, and region types are persisted
- Regions remain in "detected" state until accepted or rejected

#### FR-3: Accept/Reject Detected Regions
**Priority:** Critical  
**Description:** Mangaka must be able to review and accept/reject AI-detected regions.

**Acceptance Criteria:**
- Mangaka can accept a detected region
- Accepting creates a real PageRegion with SourceType="AI"
- AiDetectedRegion.IsAccepted is set to true
- AiDetectedRegion.PageRegionId is linked to the created PageRegion
- Mangaka can reject a detected region
- Rejecting sets IsAccepted to false without creating PageRegion
- Current user's UserId is used as CreatedByUserId
- User authorization validates Mangaka/Admin role and series access


#### FR-4: Task Suggestion Generation
**Priority:** Critical  
**Description:** Task suggestions must be generated from accepted PageRegions using valid TaskTypes from the database.

**Acceptance Criteria:**
- Suggestions are generated ONLY from accepted PageRegions, not raw AiDetectedRegions
- Region type mapping to TaskType follows these rules:
  - Panel → LayoutCleanup (fallback to Other if missing)
  - SpeechBubble → SpeechBubbleCleanup
  - Character → CleanLine or Tone
  - Background → BackgroundDrawing
  - Shading → Shading
  - Effect → Effect
  - Other → Other
- TaskTypes are loaded from database, not hardcoded
- If preferred TaskType doesn't exist, fallback to "Other"
- Suggestions have Status="Draft"
- PageRegionId references the actual accepted PageRegion
- SuggestedAssistantId is null unless valid assistant found
- DetectedRegionId is NOT used as PageRegionId

#### FR-5: Approve/Reject Task Suggestions
**Priority:** Critical  
**Description:** Mangaka must approve task suggestions before ProductionTasks are created.

**Acceptance Criteria:**
- Mangaka can approve a Draft suggestion
- Approving creates a real ProductionTask using existing task workflow
- Task status is set to Assigned
- Assistant can be selected during approval
- Deadline can be set during approval
- Suggestion Status changes to "Approved"
- Assistant is notified via INotificationService

- Mangaka can reject a Draft suggestion
- Rejecting changes Status to "Rejected" without creating task
- Assistant sees approved tasks in their task inbox

#### FR-6: Phase 0-5 Feature Stability
**Priority:** Critical  
**Description:** Existing PRN222 features must remain stable and functional.

**Acceptance Criteria:**
- All Phase 0-5 features continue to work as before
- No breaking changes to existing controllers, services, or repositories
- Existing tests pass
- DbSmokeTest passes

### Non-Functional Requirements

#### NFR-1: Architectural Compliance
**Priority:** Critical  
**Description:** Code must follow the established PRN222 layered architecture.

**Acceptance Criteria:**
- Business logic resides in Application layer services
- Infrastructure layer contains only repositories and external clients
- Controllers call Application services only, never Infrastructure or DbContext directly
- DbContext usage is isolated to Infrastructure repositories

#### NFR-2: Database Integrity
**Priority:** High  
**Description:** Database schema must enforce referential integrity and data validity.

**Acceptance Criteria:**
- All foreign key constraints are defined in DB script
- Check constraints validate data ranges (confidence 0-1, width/height > 0)
- Indexes support query performance
- Schema uses dbo. prefix consistently
- IF NOT EXISTS guards prevent duplicate constraint errors


#### NFR-3: Mock Mode Resilience
**Priority:** High  
**Description:** System must work gracefully when Python API is offline.

**Acceptance Criteria:**
- MockAiVisionClient provides fallback behavior
- Configuration allows Mode selection (Http vs Mock)
- FallbackToMock configuration enables automatic fallback
- Mock responses return realistic detection data
- No crashes when Python API is unavailable

#### NFR-4: Security
**Priority:** High  
**Description:** All state-changing operations must be protected.

**Acceptance Criteria:**
- All POST actions use anti-forgery tokens
- User authentication is required for AI Studio actions
- Authorization validates user role (Mangaka/Admin)
- Authorization validates series ownership/access

#### NFR-5: Testability
**Priority:** Medium  
**Description:** Critical workflows must have automated tests.

**Acceptance Criteria:**
- Tests cover MockAiVisionClient fallback
- Tests cover AiStudioService region type mapping
- Tests verify AcceptRegion creates PageRegion with correct CreatedByUserId
- Tests verify ApproveTaskSuggestion creates ProductionTask
- dotnet build succeeds
- dotnet test passes
- DbSmokeTest passes

---

## Design

### Architecture Overview


The AI Studio follows the PRN222 layered architecture:

```
┌─────────────────────────────────────────────────────────────┐
│  MangaWorkflow.Web (Presentation Layer)                     │
│  - AiStudioController (calls IAiStudioService)              │
│  - Views: Index, SegmentationResult, TaskSuggestions        │
│  - Anti-forgery tokens on all POST forms                    │
└──────────────────────┬──────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────────┐
│  MangaWorkflow.Application (Business Logic Layer)           │
│  - IAiStudioService / AiStudioService (orchestration)       │
│  - Uses repository interfaces only                          │
│  - No direct DbContext access                               │
└──────────────────────┬──────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────────┐
│  MangaWorkflow.Infrastructure (Data Access Layer)           │
│  - IAiInferenceRepository / AiInferenceRepository           │
│  - IAiDetectedRegionRepository / AiDetectedRegionRepository │
│  - IAiTaskSuggestionRepository / AiTaskSuggestionRepository │
│  - IAiVisionClient / HttpAiVisionClient / MockAiVisionClient│
│  - MangaWorkflowDbContext (used ONLY by repositories)       │
└──────────────────────┬──────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────────┐
│  External Python AI API (MangaWorkflow.AI)                  │
│  - FastAPI service                                           │
│  - /api/segment endpoint                                     │
│  - Mock mode with realistic detections                      │
└─────────────────────────────────────────────────────────────┘
```

### Component Design

#### 1. AiStudioService (Application Layer)

**Location:** `MangaWorkflow.Application/Services/AiStudioService.cs`  
**Interface:** `MangaWorkflow.Application/Interfaces/Services/IAiStudioService.cs`


**Responsibilities:**
- Orchestrate AI workflow (analyze → detect → suggest → approve)
- Load MangaPage from IPageRepository
- Call IAiVisionClient with image data
- Save inference results via IAiInferenceRepository
- Accept/reject detected regions (create PageRegion via IPageRegionRepository)
- Generate task suggestions from accepted regions
- Approve/reject suggestions (create ProductionTask via IProductionTaskRepository)
- Map AI region types to valid TaskTypes from database
- Validate user permissions

**Key Method Signatures:**
```csharp
Task<AiSegmentationResultDto> AnalyzePageAsync(Guid pageId, Guid requestedByUserId, string mode, CancellationToken ct);
Task AcceptRegionAsync(Guid detectedRegionId, Guid currentUserId, CancellationToken ct);
Task RejectRegionAsync(Guid detectedRegionId, Guid currentUserId, CancellationToken ct);
Task<List<AiTaskSuggestionDto>> GenerateTaskSuggestionsAsync(Guid pageId, Guid requestedByUserId, CancellationToken ct);
Task ApproveTaskSuggestionAsync(Guid suggestionId, Guid mangakaId, Guid? assistantId, DateTime? deadline, CancellationToken ct);
Task RejectTaskSuggestionAsync(Guid suggestionId, Guid mangakaId, CancellationToken ct);
```

#### 2. Repository Interfaces (Application Layer)

**New Repositories:**
- `IAiInferenceRepository` - CRUD for AiInferenceRequests
- `IAiDetectedRegionRepository` - CRUD for AiDetectedRegions
- `IAiTaskSuggestionRepository` - CRUD for AiTaskSuggestions

**Existing Repositories (reused):**
- `IPageRepository` - Load MangaPage with image data
- `IPageRegionRepository` - Create PageRegion from accepted detection
- `IProductionTaskRepository` - Create ProductionTask from approved suggestion
- `IUserRepository` - Load user and validate permissions
- `ITaskTypeRepository` - Load valid TaskTypes for mapping


#### 3. AI Vision Client (Infrastructure Layer)

**Interface:** `IAiVisionClient`  
**Implementations:**
- `HttpAiVisionClient` - Calls Python FastAPI
- `MockAiVisionClient` - Fallback when API offline

**Updated Signature:**
```csharp
Task<AiSegmentationResponseDto> SegmentPageAsync(AiSegmentationRequestDto request, CancellationToken ct = default);
```

**AiSegmentationRequestDto includes:**
- PageId (Guid)
- ImageUrl or ImagePath (string)
- Mode (string: "mock", "yolo", "sam", etc.)

#### 4. Database Schema Updates

**Foreign Key Constraints to Add:**
```sql
-- AiInferenceRequests
ALTER TABLE dbo.AiInferenceRequests 
ADD CONSTRAINT FK_AiInferenceRequests_MangaPages 
FOREIGN KEY (PageId) REFERENCES dbo.MangaPages(PageId);

ALTER TABLE dbo.AiInferenceRequests 
ADD CONSTRAINT FK_AiInferenceRequests_Users 
FOREIGN KEY (RequestedByUserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AiInferenceRequests 
ADD CONSTRAINT FK_AiInferenceRequests_ModelVersions 
FOREIGN KEY (ModelVersionId) REFERENCES dbo.AiModelVersions(ModelVersionId);

-- AiDetectedRegions
ALTER TABLE dbo.AiDetectedRegions 
ADD CONSTRAINT FK_AiDetectedRegions_InferenceRequests 
FOREIGN KEY (InferenceRequestId) REFERENCES dbo.AiInferenceRequests(InferenceRequestId);


ALTER TABLE dbo.AiDetectedRegions 
ADD CONSTRAINT FK_AiDetectedRegions_MangaPages 
FOREIGN KEY (PageId) REFERENCES dbo.MangaPages(PageId);

ALTER TABLE dbo.AiDetectedRegions 
ADD CONSTRAINT FK_AiDetectedRegions_PageRegions 
FOREIGN KEY (PageRegionId) REFERENCES dbo.PageRegions(RegionId);

-- AiTaskSuggestions
ALTER TABLE dbo.AiTaskSuggestions 
ADD CONSTRAINT FK_AiTaskSuggestions_MangaPages 
FOREIGN KEY (PageId) REFERENCES dbo.MangaPages(PageId);

ALTER TABLE dbo.AiTaskSuggestions 
ADD CONSTRAINT FK_AiTaskSuggestions_PageRegions 
FOREIGN KEY (PageRegionId) REFERENCES dbo.PageRegions(RegionId);

ALTER TABLE dbo.AiTaskSuggestions 
ADD CONSTRAINT FK_AiTaskSuggestions_Users 
FOREIGN KEY (SuggestedAssistantId) REFERENCES dbo.Users(UserId);

-- AiEvaluationMetrics
ALTER TABLE dbo.AiEvaluationMetrics 
ADD CONSTRAINT FK_AiEvaluationMetrics_Experiments 
FOREIGN KEY (ExperimentId) REFERENCES dbo.AiTrainingExperiments(ExperimentId);

ALTER TABLE dbo.AiEvaluationMetrics 
ADD CONSTRAINT FK_AiEvaluationMetrics_ModelVersions 
FOREIGN KEY (ModelVersionId) REFERENCES dbo.AiModelVersions(ModelVersionId);
```

**Check Constraints to Add:**
```sql
ALTER TABLE dbo.AiDetectedRegions 
ADD CONSTRAINT CK_AiDetectedRegions_Confidence 
CHECK (Confidence >= 0 AND Confidence <= 1);


ALTER TABLE dbo.AiDetectedRegions 
ADD CONSTRAINT CK_AiDetectedRegions_Width 
CHECK (Width > 0);

ALTER TABLE dbo.AiDetectedRegions 
ADD CONSTRAINT CK_AiDetectedRegions_Height 
CHECK (Height > 0);

ALTER TABLE dbo.AiTaskSuggestions 
ADD CONSTRAINT CK_AiTaskSuggestions_Status 
CHECK (Status IN ('Draft', 'Approved', 'Rejected'));
```

#### 5. Configuration Design

**appsettings.json Structure:**
```json
{
  "AI": {
    "Mode": "Http",
    "BaseUrl": "http://localhost:8001",
    "TimeoutSeconds": 60,
    "EnableColorization": false,
    "AutoCreateRegionsMinConfidence": 0.60,
    "RequireHumanApproval": true,
    "FallbackToMock": true
  }
}
```

**AiVisionOptions Class:**
```csharp
public class AiVisionOptions
{
    public string Mode { get; set; } = "Http";
    public string BaseUrl { get; set; } = "http://localhost:8001";
    public int TimeoutSeconds { get; set; } = 60;
    public bool EnableColorization { get; set; }
    public decimal AutoCreateRegionsMinConfidence { get; set; } = 0.60m;
    public bool RequireHumanApproval { get; set; } = true;
    public bool FallbackToMock { get; set; } = true;
}
```


#### 6. Controller Design

**AiStudioController Updates:**

**New Actions:**
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> AcceptRegion(Guid detectedRegionId);

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> RejectRegion(Guid detectedRegionId);

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> ApproveTaskSuggestion(Guid suggestionId, Guid? assistantId, DateTime? deadline);

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> RejectTaskSuggestion(Guid suggestionId);
```

#### 7. UI Design

**Index.cshtml Improvements:**
- List available pages for current Mangaka
- Show table with: Series, Chapter, Page Number, Analyze button
- Or provide dropdown of pages with analyze button

**SegmentationResult.cshtml Updates:**
- Render actual manga page image using MangaPage.ImageUrl
- Use actual image dimensions for overlay canvas
- For each detected region, show:
  - Region type badge
  - Confidence score
  - Bounding box coordinates
  - Accept/Reject status indicator
  - Accept button (POST form with anti-forgery token)
  - Reject button (POST form with anti-forgery token)


**TaskSuggestions.cshtml Updates:**
- Show suggestion cards with:
  - Title
  - Task type badge
  - Description
  - Complexity score
  - Estimated hours/amount if available
  - Status badge
  - Approve form (POST with anti-forgery token, assistant selection, deadline picker)
  - Reject button (POST with anti-forgery token)

#### 8. Python AI API Updates

**POST /api/segment Changes:**
- Accept `imagePath` or `imageUrl` in request body
- Echo back image info in response for verification
- Mock mode generates realistic detections for different region types:
  - Panel (1-6 per page)
  - SpeechBubble (2-8 per page)
  - Character (1-4 per page)
  - Background (0-2 per page)

### Data Flow Diagram

```
Mangaka → Index Page → Select Page → Analyze
                                      ↓
                        Load MangaPage (with ImageUrl)
                                      ↓
                        Create AiInferenceRequest
                                      ↓
                        Call Python API with ImageUrl
                                      ↓
                        Save AiDetectedRegions
                                      ↓
                        Show SegmentationResult (with real image)
                                      ↓
                        Mangaka reviews regions
                                      ↓
              ┌─────────────────────┴─────────────────────┐
              │                                           │
         Accept Region                              Reject Region
              │                                           │
    Create PageRegion                          Mark IsAccepted=false
    Link to Detection                                     │
              │                                           │
              └─────────────────────┬─────────────────────┘
                                    ↓
                        Generate Task Suggestions
                        (from accepted PageRegions)
                                    ↓
                        Show TaskSuggestions page
                                    ↓
                        Mangaka reviews suggestions
                                    ↓
              ┌─────────────────────┴─────────────────────┐
              │                                           │
       Approve Suggestion                          Reject Suggestion
              │                                           │
    Create ProductionTask                     Mark Status=Rejected
    Assign to Assistant                                   │
    Notify Assistant                                      │
    Mark Status=Approved                                  │
              │                                           │
              └─────────────────────┬─────────────────────┘
                                    ↓
                        Assistant sees task in inbox
```

---

## Implementation Tasks


### Task 1: Pass Real Page Image Into AI
**Priority:** Critical  
**Estimated Effort:** 4 hours

**Subtasks:**
1. Update `AiSegmentationRequestDto` to include `ImageUrl` and `ImagePath` properties
2. Update `IAiVisionClient.SegmentPageAsync` signature to accept `AiSegmentationRequestDto`
3. Modify `HttpAiVisionClient` to pass image data to Python API
4. Update Python `/api/segment` endpoint to accept `imagePath`/`imageUrl` in request
5. Modify `AiStudioService.AnalyzePageAsync` to load MangaPage from `IPageRepository`
6. Pass MangaPage.ImageUrl/FileUrl to `AiSegmentationRequestDto`
7. Update `SegmentationResult.cshtml` to use actual image URL from DTO
8. Ensure overlay canvas uses actual image dimensions

**Files to Modify:**
- `MangaWorkflow.Application/DTOs/AI/AiSegmentationRequestDto.cs`
- `MangaWorkflow.Application/Interfaces/IAiVisionClient.cs`
- `MangaWorkflow.Infrastructure/Services/HttpAiVisionClient.cs`
- `MangaWorkflow.Infrastructure/Services/MockAiVisionClient.cs`
- `MangaWorkflow.Infrastructure/Services/AiStudioService.cs` (temporary, will move in Task 2)
- `MangaWorkflow.AI/api/routers/segmentation.py`
- `MangaWorkflow.AI/api/schemas/segmentation.py`
- `MangaWorkflow.Web/Views/AiStudio/SegmentationResult.cshtml`

**Acceptance Test:**
- Navigate to AI Studio
- Select a page with actual image data
- Click Analyze
- Verify the actual manga page image is displayed (not placeholder)
- Verify overlay regions align with image

---

### Task 2: Move AiStudioService to Application Layer
**Priority:** Critical  
**Estimated Effort:** 6 hours


**Subtasks:**
1. Create `IAiInferenceRepository`, `IAiDetectedRegionRepository`, `IAiTaskSuggestionRepository` interfaces in Application
2. Implement repositories in Infrastructure using DbContext
3. Create new `MangaWorkflow.Application/Services/AiStudioService.cs`
4. Move business logic from Infrastructure service to Application service
5. Inject repository interfaces instead of DbContext
6. Remove all direct DbContext access from Application service
7. Update DI registration in `MangaWorkflow.Application/DependencyInjection.cs`
8. Update DI registration in `MangaWorkflow.Infrastructure/DependencyInjection.cs`
9. Delete old `MangaWorkflow.Infrastructure/Services/AiStudioService.cs`
10. Verify controller still works with new Application service

**Files to Create:**
- `MangaWorkflow.Application/Interfaces/Repositories/IAiInferenceRepository.cs`
- `MangaWorkflow.Application/Interfaces/Repositories/IAiDetectedRegionRepository.cs`
- `MangaWorkflow.Application/Interfaces/Repositories/IAiTaskSuggestionRepository.cs`
- `MangaWorkflow.Infrastructure/Repositories/AiInferenceRepository.cs`
- `MangaWorkflow.Infrastructure/Repositories/AiDetectedRegionRepository.cs`
- `MangaWorkflow.Infrastructure/Repositories/AiTaskSuggestionRepository.cs`
- `MangaWorkflow.Application/Services/AiStudioService.cs`

**Files to Modify:**
- `MangaWorkflow.Application/Interfaces/Services/IAiStudioService.cs` (ensure it's here, not Infrastructure)
- `MangaWorkflow.Application/DependencyInjection.cs`
- `MangaWorkflow.Infrastructure/DependencyInjection.cs`

**Files to Delete:**
- `MangaWorkflow.Infrastructure/Services/AiStudioService.cs`

**Acceptance Test:**
- Run `dotnet build`
- Verify no compilation errors
- Navigate to AI Studio
- Verify page analysis still works
- Check that DbContext is never directly injected into Application services

---

### Task 3: Add Accept/Reject Detected Region Flow
**Priority:** Critical  
**Estimated Effort:** 5 hours

**Subtasks:**
1. Update `IAiStudioService.AcceptRegionAsync` signature to include `currentUserId` parameter
2. Update `IAiStudioService.RejectRegionAsync` signature to include `currentUserId` parameter
3. Implement user authorization check (Mangaka/Admin role, series access validation)
4. In `AcceptRegionAsync`: set `IsAccepted=true`, create PageRegion via `IPageRegionRepository`, set `SourceType="AI"`, link `PageRegionId`
5. In `AcceptRegionAsync`: use `currentUserId` as `CreatedByUserId`
6. In `RejectRegionAsync`: set `IsAccepted=false`
7. Add `AcceptRegion` POST action in `AiStudioController` with anti-forgery validation
8. Add `RejectRegion` POST action in `AiStudioController` with anti-forgery validation
9. Get current user ID from `User.Claims` or authentication context
10. Update `SegmentationResult.cshtml` to add Accept/Reject forms for each region
11. Add anti-forgery tokens to forms
12. Show accept/reject status badges
13. Redirect back to segmentation result after action

**Files to Modify:**
- `MangaWorkflow.Application/Interfaces/Services/IAiStudioService.cs`
- `MangaWorkflow.Application/Services/AiStudioService.cs`
- `MangaWorkflow.Web/Controllers/AiStudioController.cs`
- `MangaWorkflow.Web/Views/AiStudio/SegmentationResult.cshtml`

**Acceptance Test:**
- Analyze a page
- Click Accept on a detected region
- Verify region status changes to Accepted
- Verify a PageRegion is created in database with SourceType="AI"
- Verify CreatedByUserId matches current user
- Click Reject on another region
- Verify region status changes to Rejected
- Verify no PageRegion is created

---

### Task 4: Fix Task Suggestion Logic
**Priority:** Critical  
**Estimated Effort:** 5 hours


**Subtasks:**
1. Modify `GenerateTaskSuggestionsAsync` to query only accepted PageRegions (where AiDetectedRegion.IsAccepted=true)
2. Load all valid TaskTypes from database via repository
3. Implement region type to TaskType mapping:
   - Panel → LayoutCleanup (fallback Other)
   - SpeechBubble → SpeechBubbleCleanup
   - Character → CleanLine or Tone
   - Background → BackgroundDrawing
   - Shading → Shading
   - Effect → Effect
   - Other → Other
4. Use fallback to "Other" if preferred TaskType doesn't exist
5. Set suggestion `Status="Draft"`
6. Set `PageRegionId` to the actual accepted PageRegion ID (not DetectedRegionId)
7. Set `SuggestedAssistantId=null` unless valid assistant found
8. Update Python mock suggester to return more region types if needed

**Files to Modify:**
- `MangaWorkflow.Application/Services/AiStudioService.cs`
- `MangaWorkflow.AI/api/services/task_suggester.py` (optional)

**Acceptance Test:**
- Accept multiple regions of different types (Panel, SpeechBubble, Character)
- Generate task suggestions
- Verify suggestions are created only for accepted regions
- Verify task types match the mapping rules
- Verify PageRegionId references actual PageRegion, not DetectedRegion
- Verify all suggestions have Status="Draft"

---

### Task 5: Add Approve/Reject Task Suggestion Flow
**Priority:** Critical  
**Estimated Effort:** 6 hours

**Subtasks:**
1. Implement `ApproveTaskSuggestionAsync` in `AiStudioService`
2. Validate suggestion status is "Draft"
3. Load PageRegion and validate it exists
4. Create ProductionTask via `IProductionTaskRepository` using existing task workflow
5. Set task status to "Assigned"
6. Set assigned assistant if provided
7. Set deadline if provided
8. Mark suggestion Status="Approved"
9. Notify assistant via `INotificationService.CreateAndSendAsync`
10. Implement `RejectTaskSuggestionAsync` to mark Status="Rejected"
11. Add `ApproveTaskSuggestion` POST action in controller
12. Add `RejectTaskSuggestion` POST action in controller
13. Update `TaskSuggestions.cshtml` with Approve/Reject forms
14. Add assistant selection dropdown in approve form
15. Add deadline date picker in approve form
16. Include anti-forgery tokens


**Files to Modify:**
- `MangaWorkflow.Application/Interfaces/Services/IAiStudioService.cs`
- `MangaWorkflow.Application/Services/AiStudioService.cs`
- `MangaWorkflow.Web/Controllers/AiStudioController.cs`
- `MangaWorkflow.Web/Views/AiStudio/TaskSuggestions.cshtml`

**Acceptance Test:**
- Generate task suggestions
- Navigate to TaskSuggestions page
- Approve a suggestion with assistant selection
- Verify ProductionTask is created
- Verify task status is "Assigned"
- Verify assistant is set
- Verify notification is sent
- Log in as Assistant
- Verify task appears in Assistant's task inbox
- Go back to TaskSuggestions
- Reject another suggestion
- Verify status changes to "Rejected"
- Verify no ProductionTask is created

---

### Task 6: Fix AI DB Script Integrity
**Priority:** High  
**Estimated Effort:** 3 hours

**Subtasks:**
1. Add dbo. schema prefix consistently to all table references
2. Add foreign key constraint: `AiInferenceRequests.PageId → MangaPages(PageId)`
3. Add foreign key constraint: `AiInferenceRequests.RequestedByUserId → Users(UserId)`
4. Add foreign key constraint: `AiInferenceRequests.ModelVersionId → AiModelVersions(ModelVersionId)`
5. Add foreign key constraint: `AiDetectedRegions.InferenceRequestId → AiInferenceRequests(InferenceRequestId)`
6. Add foreign key constraint: `AiDetectedRegions.PageId → MangaPages(PageId)`
7. Add foreign key constraint: `AiDetectedRegions.PageRegionId → PageRegions(RegionId)`
8. Add foreign key constraint: `AiTaskSuggestions.PageId → MangaPages(PageId)`
9. Add foreign key constraint: `AiTaskSuggestions.PageRegionId → PageRegions(RegionId)`
10. Add foreign key constraint: `AiTaskSuggestions.SuggestedAssistantId → Users(UserId)`
11. Add foreign key constraint: `AiEvaluationMetrics.ExperimentId → AiTrainingExperiments(ExperimentId)`
12. Add foreign key constraint: `AiEvaluationMetrics.ModelVersionId → AiModelVersions(ModelVersionId)`
13. Add check constraint: `AiDetectedRegions.Confidence BETWEEN 0 AND 1`
14. Add check constraint: `AiDetectedRegions.Width > 0`
15. Add check constraint: `AiDetectedRegions.Height > 0`
16. Add check constraint: `AiTaskSuggestions.Status IN ('Draft', 'Approved', 'Rejected')`
17. Use `IF NOT EXISTS` guards for all constraints


**Files to Modify:**
- `db/MangaWorkflowDB_v4_ai_extension.sql`

**Acceptance Test:**
- Run the v4 script on a fresh v3 database
- Verify all tables are created
- Verify all foreign keys exist
- Verify all check constraints exist
- Verify referential integrity is enforced (test with invalid FK values)
- Run DbSmokeTest to verify schema integrity

---

### Task 7: Fix AI Configuration
**Priority:** High  
**Estimated Effort:** 2 hours

**Subtasks:**
1. Add AI configuration section to `appsettings.json`
2. Create `AiVisionOptions` class in Application or Infrastructure
3. Register options in DI using `services.Configure<AiVisionOptions>`
4. Update `HttpAiVisionClient` to read `AI:BaseUrl` instead of `AiApi:BaseUrl`
5. Implement fallback to `MockAiVisionClient` when `FallbackToMock=true` and HTTP fails
6. Add configuration validation on startup

**Files to Modify:**
- `MangaWorkflow.Web/appsettings.json`
- `MangaWorkflow.Application/Options/AiVisionOptions.cs` (create)
- `MangaWorkflow.Infrastructure/DependencyInjection.cs`
- `MangaWorkflow.Infrastructure/Services/HttpAiVisionClient.cs`

**Acceptance Test:**
- Start application with Python API offline
- Navigate to AI Studio
- Analyze a page
- Verify mock fallback works
- Start Python API
- Analyze another page
- Verify HTTP mode works
- Check logs for proper configuration reading

---

### Task 8: Improve AI Studio UI Flow
**Priority:** Medium  
**Estimated Effort:** 4 hours


**Subtasks:**
1. Update `Index` action to load available pages for current Mangaka
2. Display pages in a table or dropdown with columns: Series, Chapter, Page Number, Analyze button
3. Update `SegmentationResult.cshtml` to show region details in cards or table
4. Display region type badge (color-coded)
5. Display confidence score with progress bar
6. Display bounding box coordinates
7. Show accept/reject status with icon/badge
8. Style Accept/Reject buttons consistently
9. Update `TaskSuggestions.cshtml` to show suggestions in cards
10. Display task type badge
11. Display complexity and estimated hours/amount
12. Add assistant selection dropdown with actual assistants loaded from database
13. Add deadline date picker
14. Style forms consistently with Bootstrap or existing UI framework

**Files to Modify:**
- `MangaWorkflow.Web/Controllers/AiStudioController.cs`
- `MangaWorkflow.Web/Views/AiStudio/Index.cshtml`
- `MangaWorkflow.Web/Views/AiStudio/SegmentationResult.cshtml`
- `MangaWorkflow.Web/Views/AiStudio/TaskSuggestions.cshtml`

**Acceptance Test:**
- Navigate to AI Studio Index
- Verify pages are listed (not manual ID entry)
- Select a page and analyze
- Verify UI clearly shows region details and actions
- Verify accept/reject buttons are prominent and clear
- Navigate to TaskSuggestions
- Verify suggestions are presented clearly
- Verify approve form has assistant dropdown and deadline picker

---

### Task 9: Update Training Docs for Honesty
**Priority:** Medium  
**Estimated Effort:** 2 hours

**Subtasks:**
1. Update `AI_MODEL_TRAINING_GUIDE.md` to clarify mock mode status
2. State that real YOLO training is not yet completed
3. Document that Manga109 must be manually provided
4. Note that `prepare_manga109.py` validates/prepares structure (not full training-ready conversion yet)
5. Create placeholder scripts if missing:
   - `training/evaluate_segmentation.py`
   - `training/export_onnx.py`
   - `training/compare_models.py`
   - `training/train_unet.py`
6. Mark placeholders as "scaffold" or "dry-run" in comments
7. Remove any fake mAP/IoU metrics from docs
8. Add disclaimer that metrics shown are from mock data only


**Files to Modify:**
- `MangaWorkflow.AI/AI_MODEL_TRAINING_GUIDE.md`
- `MangaWorkflow.AI/AI_DATASET_PREPARATION_GUIDE.md`
- `MangaWorkflow.AI/README.md`

**Files to Create (placeholders):**
- `MangaWorkflow.AI/training/evaluate_segmentation.py`
- `MangaWorkflow.AI/training/export_onnx.py`
- `MangaWorkflow.AI/training/compare_models.py`
- `MangaWorkflow.AI/training/train_unet.py`

**Acceptance Test:**
- Review updated documentation
- Verify no fake metrics are claimed
- Verify mock mode is clearly documented
- Verify placeholder scripts have appropriate comments

---

### Task 10: Add Verification and Tests
**Priority:** High  
**Estimated Effort:** 5 hours

**Subtasks:**
1. Add unit test for `MockAiVisionClient` fallback behavior
2. Add unit test for `AiStudioService` region type mapping (Panel→LayoutCleanup, etc.)
3. Add unit test verifying `AcceptRegion` creates PageRegion with correct `CreatedByUserId`
4. Add unit test verifying `ApproveTaskSuggestion` creates ProductionTask
5. Add integration test for full workflow if feasible
6. Run `dotnet clean`
7. Run `dotnet restore`
8. Run `dotnet build` (verify success)
9. Run `dotnet test` (verify all tests pass)
10. Run `dotnet run --project MangaWorkflow.Tools.DbSmokeTest` (verify DB integrity)
11. Test Python API:
    - Install `requirements-api.txt`
    - Start API: `uvicorn api.main:app --host 0.0.0.0 --port 8001`
    - Test `GET /health`
    - Test `POST /api/segment` with sample payload

**Files to Create:**
- `MangaWorkflow.Tests/Application/Services/AiStudioServiceTests.cs`
- `MangaWorkflow.Tests/Infrastructure/Services/MockAiVisionClientTests.cs`

**Files to Modify:**
- `MangaWorkflow.Tests/MangaWorkflow.Tests.csproj` (add test dependencies if needed)


**Acceptance Test:**
- All unit tests pass
- All integration tests pass
- `dotnet build` succeeds with zero errors
- DbSmokeTest passes
- Python API health check passes
- Python API segmentation endpoint responds correctly

---

## Testing Strategy

### Unit Tests

**Test Coverage:**
- MockAiVisionClient returns fallback data when HTTP fails
- Region type mapping logic (Panel→LayoutCleanup, SpeechBubble→SpeechBubbleCleanup, etc.)
- TaskType fallback to "Other" when preferred type missing
- AcceptRegion sets CreatedByUserId from currentUserId parameter
- AcceptRegion creates PageRegion with SourceType="AI"
- RejectRegion sets IsAccepted=false without creating PageRegion
- ApproveTaskSuggestion creates ProductionTask with correct properties
- ApproveTaskSuggestion marks suggestion as "Approved"
- RejectTaskSuggestion marks suggestion as "Rejected"

### Integration Tests

**Test Scenarios:**
1. Full workflow: Analyze → Detect → Accept → Suggest → Approve → Verify Task Created
2. Python API offline: Verify mock fallback works
3. Database constraints: Test FK violations are prevented
4. Authorization: Non-Mangaka cannot accept regions

### Manual Verification

**Checklist:**
- [ ] AI Studio Index shows page list, not manual ID entry
- [ ] Analyze page shows actual manga image, not placeholder
- [ ] Detected regions overlay correctly on image
- [ ] Accept region creates PageRegion in database
- [ ] Reject region updates IsAccepted flag
- [ ] Task suggestions generated only from accepted regions
- [ ] Task types match database TaskTypes
- [ ] Approve suggestion creates ProductionTask
- [ ] Assistant sees task in inbox
- [ ] All POST actions protected with anti-forgery tokens
- [ ] Existing Phase 0-5 features still work
- [ ] DbSmokeTest passes
- [ ] Python API health check passes

---

## Acceptance Criteria Summary


This spec is considered complete when ALL of the following are true:

### Functional
- [x] Existing PRN222 app builds without errors
- [ ] Existing PRN222 Phase 0-5 features work as before
- [ ] AI Studio shows real manga page image, not placeholder
- [ ] Python API mock mode returns realistic detections
- [ ] .NET sends actual image URL/path to Python API
- [ ] Detected regions are saved as AiDetectedRegions
- [ ] Mangaka can accept detected regions
- [ ] Mangaka can reject detected regions
- [ ] Accepted regions become real PageRegions with SourceType="AI"
- [ ] Task suggestions are generated from accepted PageRegions only
- [ ] Suggestions use valid TaskTypes from database
- [ ] Suggestions fallback to "Other" when preferred TaskType missing
- [ ] Mangaka can approve task suggestions
- [ ] Approved suggestions create real ProductionTasks
- [ ] Assistant sees approved tasks in task inbox
- [ ] Mangaka can reject task suggestions

### Architectural
- [ ] AiStudioService business logic resides in Application layer
- [ ] Infrastructure contains only repositories and external clients
- [ ] Controllers call Application services only (no Infrastructure or DbContext)
- [ ] DbContext usage isolated to Infrastructure repositories
- [ ] Repository interfaces defined in Application layer
- [ ] Repository implementations in Infrastructure layer

### Security
- [ ] All POST actions use anti-forgery tokens
- [ ] User authentication required for AI Studio
- [ ] User authorization validates Mangaka/Admin role
- [ ] User authorization validates series access

### Data Integrity
- [ ] DB v4 script has all foreign key constraints
- [ ] DB v4 script has check constraints (confidence, width, height, status)
- [ ] DB v4 script uses dbo. schema consistently
- [ ] DB v4 script uses IF NOT EXISTS guards


### Configuration
- [ ] AI configuration section exists in appsettings.json
- [ ] AiVisionOptions class created and registered
- [ ] DI reads AI:BaseUrl (not AiApi:BaseUrl)
- [ ] FallbackToMock configuration works
- [ ] Mock mode works when Python API offline

### Documentation
- [ ] Docs clearly state mock mode status
- [ ] Docs state real YOLO training not yet completed
- [ ] No fake mAP/IoU metrics claimed
- [ ] Training script placeholders marked as scaffold/dry-run

### Testing & Verification
- [ ] `dotnet clean` succeeds
- [ ] `dotnet restore` succeeds
- [ ] `dotnet build` succeeds with zero errors
- [ ] `dotnet test` succeeds with all tests passing
- [ ] `dotnet run --project MangaWorkflow.Tools.DbSmokeTest` passes
- [ ] Python API `GET /health` responds 200 OK
- [ ] Python API `POST /api/segment` responds with mock detections
- [ ] Unit tests for MockAiVisionClient fallback
- [ ] Unit tests for region type mapping
- [ ] Unit tests for AcceptRegion with CreatedByUserId
- [ ] Unit tests for ApproveTaskSuggestion creating ProductionTask

---

## Risk Assessment

### High Risk
- **Architectural refactoring may break existing features**
  - Mitigation: Comprehensive testing after each task, maintain existing tests
- **Database constraint additions may conflict with existing data**
  - Mitigation: Use IF NOT EXISTS guards, test on copy of production data

### Medium Risk
- **Python API offline may cause poor user experience**
  - Mitigation: Robust mock fallback, clear error messages
- **Image URL/path may not be available for all pages**
  - Mitigation: Validate image exists before analysis, show helpful error

### Low Risk
- **UI changes may not match design expectations**
  - Mitigation: Iterate with user feedback, use existing UI patterns

---

## Dependencies


### External Dependencies
- Python 3.9+ (for AI API)
- FastAPI, uvicorn (Python packages)
- SQL Server (for database)
- .NET 8.0 SDK

### Internal Dependencies
- Existing PRN222 Phase 0-5 code must be stable
- Database v3 must be applied before v4
- MangaPage entities must have ImageUrl or FileUrl populated
- TaskType reference data must exist in database
- User authentication/authorization system must be functional
- INotificationService must be available for assistant notifications

---

## Rollback Plan

If critical issues arise after deployment:

1. **Immediate Rollback:**
   - Revert branch to previous stable commit
   - Restore database to pre-v4 backup
   - Restart application

2. **Partial Rollback:**
   - Disable AI Studio feature via configuration flag
   - Keep existing features functional
   - Fix issues in separate hotfix branch

3. **Data Preservation:**
   - Export AI-related data before rollback
   - Preserve for analysis and future retry

---

## Success Metrics

### Functional Success
- 100% of acceptance criteria met
- Zero critical bugs in AI Studio workflow
- Existing Phase 0-5 features maintain 100% functionality

### Code Quality Success
- Zero architectural violations (DbContext in Application layer)
- 100% of unit tests passing
- Code review approval from team

### User Experience Success
- Mangaka can complete full workflow (analyze → accept → approve) in < 5 minutes
- UI is intuitive without documentation
- Error messages are clear and actionable

---

## Timeline Estimate


| Task | Priority | Estimated Effort | Status |
|------|----------|------------------|--------|
| Task 1: Pass Real Page Image Into AI | Critical | 4 hours | Pending |
| Task 2: Move AiStudioService to Application | Critical | 6 hours | Pending |
| Task 3: Add Accept/Reject Region Flow | Critical | 5 hours | Pending |
| Task 4: Fix Task Suggestion Logic | Critical | 5 hours | Pending |
| Task 5: Add Approve/Reject Suggestion Flow | Critical | 6 hours | Pending |
| Task 6: Fix AI DB Script Integrity | High | 3 hours | Pending |
| Task 7: Fix AI Configuration | High | 2 hours | Pending |
| Task 8: Improve AI Studio UI Flow | Medium | 4 hours | Pending |
| Task 9: Update Training Docs | Medium | 2 hours | Pending |
| Task 10: Add Verification and Tests | High | 5 hours | Pending |
| **Total** | | **42 hours** | |

**Estimated Duration:** 1-2 weeks (with 1-2 developers)

---

## Merge Readiness Criteria

The branch `feature/ai-studio-fullstack` is ready to merge to `main` when:

1. ✅ All 10 implementation tasks are completed
2. ✅ All acceptance criteria are met
3. ✅ All automated tests pass
4. ✅ DbSmokeTest passes
5. ✅ Manual verification checklist is 100% complete
6. ✅ Code review approved
7. ✅ No critical or high-severity bugs outstanding
8. ✅ Python API mock mode verified working
9. ✅ Documentation updated and accurate
10. ✅ Existing Phase 0-5 features verified stable

**DO NOT MERGE** until all criteria above are satisfied.

---

## Appendix: File Map

### Files to Create
```
MangaWorkflow.Application/
  Services/AiStudioService.cs
  Interfaces/Repositories/IAiInferenceRepository.cs
  Interfaces/Repositories/IAiDetectedRegionRepository.cs
  Interfaces/Repositories/IAiTaskSuggestionRepository.cs
  Options/AiVisionOptions.cs

MangaWorkflow.Infrastructure/
  Repositories/AiInferenceRepository.cs
  Repositories/AiDetectedRegionRepository.cs
  Repositories/AiTaskSuggestionRepository.cs

MangaWorkflow.Tests/
  Application/Services/AiStudioServiceTests.cs
  Infrastructure/Services/MockAiVisionClientTests.cs

MangaWorkflow.AI/training/
  evaluate_segmentation.py
  export_onnx.py
  compare_models.py
  train_unet.py
```


### Files to Modify
```
db/
  MangaWorkflowDB_v4_ai_extension.sql

MangaWorkflow.Application/
  DTOs/AI/AiSegmentationRequestDto.cs
  Interfaces/IAiVisionClient.cs
  Interfaces/Services/IAiStudioService.cs
  DependencyInjection.cs

MangaWorkflow.Infrastructure/
  Services/HttpAiVisionClient.cs
  Services/MockAiVisionClient.cs
  DependencyInjection.cs

MangaWorkflow.Web/
  Controllers/AiStudioController.cs
  Views/AiStudio/Index.cshtml
  Views/AiStudio/SegmentationResult.cshtml
  Views/AiStudio/TaskSuggestions.cshtml
  appsettings.json

MangaWorkflow.AI/
  api/routers/segmentation.py
  api/schemas/segmentation.py
  API_MODEL_TRAINING_GUIDE.md
  AI_DATASET_PREPARATION_GUIDE.md
  README.md
```

### Files to Delete
```
MangaWorkflow.Infrastructure/
  Services/AiStudioService.cs  (moved to Application layer)
```

---

## Document History

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | 2026-06-27 | Kiro AI | Initial specification created |

---

## Approval

**Specification Status:** ✅ Ready for Implementation

**Approved By:** Pending  
**Approval Date:** Pending  

Once approved, implementation should proceed task-by-task in the order specified, with verification after each major task group.

---

**END OF SPECIFICATION**
