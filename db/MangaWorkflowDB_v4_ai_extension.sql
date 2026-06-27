-- MangaWorkflowDB_v4_ai_extension.sql
-- Additive script for AI Studio Extension

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AiModelVersions')
BEGIN
    CREATE TABLE AiModelVersions (
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
    CREATE TABLE AiInferenceRequests (
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

    CREATE INDEX IX_AiInferenceRequests_PageId_Status ON AiInferenceRequests(PageId, Status);
END

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AiDetectedRegions')
BEGIN
    CREATE TABLE AiDetectedRegions (
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

    CREATE INDEX IX_AiDetectedRegions_PageId ON AiDetectedRegions(PageId);
END

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AiTaskSuggestions')
BEGIN
    CREATE TABLE AiTaskSuggestions (
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

    CREATE INDEX IX_AiTaskSuggestions_PageId_Status ON AiTaskSuggestions(PageId, Status);
END

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AiTrainingExperiments')
BEGIN
    CREATE TABLE AiTrainingExperiments (
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
    CREATE TABLE AiEvaluationMetrics (
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

    CREATE INDEX IX_AiEvaluationMetrics_ModelVersionId ON AiEvaluationMetrics(ModelVersionId);
END
