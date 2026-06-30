-- MangaWorkflowDB_v4_ai_extension.sql
-- Additive script for AI Studio Extension

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AiModelVersions')
BEGIN
    CREATE TABLE dbo.AiModelVersions (
        ModelVersionId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        ModelName NVARCHAR(100) NOT NULL,
        ModelType VARCHAR(50) NOT NULL,
        VersionLabel NVARCHAR(100) NOT NULL,
        Framework VARCHAR(50) NOT NULL,
        ModelPath NVARCHAR(1000) NULL,
        IsActive BIT NOT NULL DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        Notes NVARCHAR(MAX) NULL
    );
END

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AiInferenceRequests')
BEGIN
    CREATE TABLE dbo.AiInferenceRequests (
        InferenceRequestId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        PageId UNIQUEIDENTIFIER NOT NULL,
        RequestedByUserId UNIQUEIDENTIFIER NOT NULL,
        ModelVersionId UNIQUEIDENTIFIER NULL,
        RequestType VARCHAR(50) NOT NULL,
        Status VARCHAR(50) NOT NULL,
        StartedAt DATETIME2 NULL,
        FinishedAt DATETIME2 NULL,
        ErrorMessage NVARCHAR(MAX) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );

    CREATE INDEX IX_AiInferenceRequests_PageId_Status ON dbo.AiInferenceRequests(PageId, Status);
END

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AiDetectedRegions')
BEGIN
    CREATE TABLE dbo.AiDetectedRegions (
        DetectedRegionId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        InferenceRequestId UNIQUEIDENTIFIER NOT NULL,
        PageId UNIQUEIDENTIFIER NOT NULL,
        PageRegionId UNIQUEIDENTIFIER NULL,
        RegionTypeCode VARCHAR(50) NOT NULL,
        X DECIMAL(10,2) NOT NULL,
        Y DECIMAL(10,2) NOT NULL,
        Width DECIMAL(10,2) NOT NULL,
        Height DECIMAL(10,2) NOT NULL,
        Confidence DECIMAL(5,4) NOT NULL,
        PolygonJson NVARCHAR(MAX) NULL,
        IsAccepted BIT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );

    CREATE INDEX IX_AiDetectedRegions_PageId ON dbo.AiDetectedRegions(PageId);
END

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AiTaskSuggestions')
BEGIN
    CREATE TABLE dbo.AiTaskSuggestions (
        TaskSuggestionId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        PageId UNIQUEIDENTIFIER NOT NULL,
        PageRegionId UNIQUEIDENTIFIER NULL,
        SuggestedAssistantId UNIQUEIDENTIFIER NULL,
        TaskTypeCode VARCHAR(50) NOT NULL,
        Title NVARCHAR(200) NOT NULL,
        Description NVARCHAR(MAX) NULL,
        ComplexityScore DECIMAL(5,2) NOT NULL DEFAULT 1,
        EstimatedHours DECIMAL(6,2) NULL,
        EstimatedAmount DECIMAL(12,2) NULL,
        Status VARCHAR(50) NOT NULL DEFAULT 'Draft',
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );

    CREATE INDEX IX_AiTaskSuggestions_PageId_Status ON dbo.AiTaskSuggestions(PageId, Status);
END

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AiTrainingExperiments')
BEGIN
    CREATE TABLE dbo.AiTrainingExperiments (
        ExperimentId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        ExperimentName NVARCHAR(200) NOT NULL,
        ModelType VARCHAR(50) NOT NULL,
        DatasetName NVARCHAR(100) NOT NULL,
        ConfigJson NVARCHAR(MAX) NULL,
        Status VARCHAR(50) NOT NULL,
        StartedAt DATETIME2 NULL,
        FinishedAt DATETIME2 NULL,
        Notes NVARCHAR(MAX) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
END

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AiEvaluationMetrics')
BEGIN
    CREATE TABLE dbo.AiEvaluationMetrics (
        MetricId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        ExperimentId UNIQUEIDENTIFIER NULL,
        ModelVersionId UNIQUEIDENTIFIER NULL,
        ClassName VARCHAR(50) NOT NULL,
        IoU DECIMAL(6,4) NULL,
        DiceF1 DECIMAL(6,4) NULL,
        PrecisionValue DECIMAL(6,4) NULL,
        RecallValue DECIMAL(6,4) NULL,
        Map50 DECIMAL(6,4) NULL,
        Map5095 DECIMAL(6,4) NULL,
        LatencyMs DECIMAL(10,2) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );

    CREATE INDEX IX_AiEvaluationMetrics_ModelVersionId ON dbo.AiEvaluationMetrics(ModelVersionId);
END

-- ========================================
-- FOREIGN KEY CONSTRAINTS
-- ========================================

-- AiInferenceRequests Foreign Keys
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AiInferenceRequests_MangaPages')
BEGIN
    ALTER TABLE dbo.AiInferenceRequests 
    ADD CONSTRAINT FK_AiInferenceRequests_MangaPages 
    FOREIGN KEY (PageId) REFERENCES dbo.MangaPages(PageId);
END

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AiInferenceRequests_Users')
BEGIN
    ALTER TABLE dbo.AiInferenceRequests 
    ADD CONSTRAINT FK_AiInferenceRequests_Users 
    FOREIGN KEY (RequestedByUserId) REFERENCES dbo.Users(UserId);
END

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AiInferenceRequests_ModelVersions')
BEGIN
    ALTER TABLE dbo.AiInferenceRequests 
    ADD CONSTRAINT FK_AiInferenceRequests_ModelVersions 
    FOREIGN KEY (ModelVersionId) REFERENCES dbo.AiModelVersions(ModelVersionId);
END

-- AiDetectedRegions Foreign Keys
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AiDetectedRegions_InferenceRequests')
BEGIN
    ALTER TABLE dbo.AiDetectedRegions 
    ADD CONSTRAINT FK_AiDetectedRegions_InferenceRequests 
    FOREIGN KEY (InferenceRequestId) REFERENCES dbo.AiInferenceRequests(InferenceRequestId);
END

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AiDetectedRegions_MangaPages')
BEGIN
    ALTER TABLE dbo.AiDetectedRegions 
    ADD CONSTRAINT FK_AiDetectedRegions_MangaPages 
    FOREIGN KEY (PageId) REFERENCES dbo.MangaPages(PageId);
END

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AiDetectedRegions_PageRegions')
BEGIN
    ALTER TABLE dbo.AiDetectedRegions 
    ADD CONSTRAINT FK_AiDetectedRegions_PageRegions 
    FOREIGN KEY (PageRegionId) REFERENCES dbo.PageRegions(RegionId);
END

-- AiTaskSuggestions Foreign Keys
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AiTaskSuggestions_MangaPages')
BEGIN
    ALTER TABLE dbo.AiTaskSuggestions 
    ADD CONSTRAINT FK_AiTaskSuggestions_MangaPages 
    FOREIGN KEY (PageId) REFERENCES dbo.MangaPages(PageId);
END

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AiTaskSuggestions_PageRegions')
BEGIN
    ALTER TABLE dbo.AiTaskSuggestions 
    ADD CONSTRAINT FK_AiTaskSuggestions_PageRegions 
    FOREIGN KEY (PageRegionId) REFERENCES dbo.PageRegions(RegionId);
END

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AiTaskSuggestions_Users')
BEGIN
    ALTER TABLE dbo.AiTaskSuggestions 
    ADD CONSTRAINT FK_AiTaskSuggestions_Users 
    FOREIGN KEY (SuggestedAssistantId) REFERENCES dbo.Users(UserId);
END

-- AiEvaluationMetrics Foreign Keys
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AiEvaluationMetrics_Experiments')
BEGIN
    ALTER TABLE dbo.AiEvaluationMetrics 
    ADD CONSTRAINT FK_AiEvaluationMetrics_Experiments 
    FOREIGN KEY (ExperimentId) REFERENCES dbo.AiTrainingExperiments(ExperimentId);
END

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AiEvaluationMetrics_ModelVersions')
BEGIN
    ALTER TABLE dbo.AiEvaluationMetrics 
    ADD CONSTRAINT FK_AiEvaluationMetrics_ModelVersions 
    FOREIGN KEY (ModelVersionId) REFERENCES dbo.AiModelVersions(ModelVersionId);
END

-- ========================================
-- CHECK CONSTRAINTS
-- ========================================

-- AiDetectedRegions Check Constraints
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_AiDetectedRegions_Confidence')
BEGIN
    ALTER TABLE dbo.AiDetectedRegions 
    ADD CONSTRAINT CK_AiDetectedRegions_Confidence 
    CHECK (Confidence >= 0 AND Confidence <= 1);
END

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_AiDetectedRegions_Width')
BEGIN
    ALTER TABLE dbo.AiDetectedRegions 
    ADD CONSTRAINT CK_AiDetectedRegions_Width 
    CHECK (Width > 0);
END

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_AiDetectedRegions_Height')
BEGIN
    ALTER TABLE dbo.AiDetectedRegions 
    ADD CONSTRAINT CK_AiDetectedRegions_Height 
    CHECK (Height > 0);
END

-- AiTaskSuggestions Check Constraints
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_AiTaskSuggestions_Status')
BEGIN
    ALTER TABLE dbo.AiTaskSuggestions 
    ADD CONSTRAINT CK_AiTaskSuggestions_Status 
    CHECK (Status IN ('Draft', 'Approved', 'Rejected'));
END
