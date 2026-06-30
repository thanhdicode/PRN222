-- test_v4_integrity.sql
-- Script to validate MangaWorkflowDB_v4_ai_extension.sql integrity
-- This script checks that all constraints are properly created

USE MangaWorkflowDB;
GO

PRINT '========================================';
PRINT 'Testing MangaWorkflowDB_v4_ai_extension.sql Integrity';
PRINT '========================================';
PRINT '';

-- Check Tables Exist
PRINT 'Checking Tables...';
DECLARE @missingTables INT = 0;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AiModelVersions')
BEGIN
    PRINT '  ✗ AiModelVersions table is MISSING';
    SET @missingTables = @missingTables + 1;
END
ELSE
    PRINT '  ✓ AiModelVersions table exists';

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AiInferenceRequests')
BEGIN
    PRINT '  ✗ AiInferenceRequests table is MISSING';
    SET @missingTables = @missingTables + 1;
END
ELSE
    PRINT '  ✓ AiInferenceRequests table exists';

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AiDetectedRegions')
BEGIN
    PRINT '  ✗ AiDetectedRegions table is MISSING';
    SET @missingTables = @missingTables + 1;
END
ELSE
    PRINT '  ✓ AiDetectedRegions table exists';

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AiTaskSuggestions')
BEGIN
    PRINT '  ✗ AiTaskSuggestions table is MISSING';
    SET @missingTables = @missingTables + 1;
END
ELSE
    PRINT '  ✓ AiTaskSuggestions table exists';

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AiTrainingExperiments')
BEGIN
    PRINT '  ✗ AiTrainingExperiments table is MISSING';
    SET @missingTables = @missingTables + 1;
END
ELSE
    PRINT '  ✓ AiTrainingExperiments table exists';

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AiEvaluationMetrics')
BEGIN
    PRINT '  ✗ AiEvaluationMetrics table is MISSING';
    SET @missingTables = @missingTables + 1;
END
ELSE
    PRINT '  ✓ AiEvaluationMetrics table exists';

PRINT '';

-- Check Foreign Key Constraints
PRINT 'Checking Foreign Key Constraints...';
DECLARE @missingFKs INT = 0;

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AiInferenceRequests_MangaPages')
BEGIN
    PRINT '  ✗ FK_AiInferenceRequests_MangaPages is MISSING';
    SET @missingFKs = @missingFKs + 1;
END
ELSE
    PRINT '  ✓ FK_AiInferenceRequests_MangaPages exists';

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AiInferenceRequests_Users')
BEGIN
    PRINT '  ✗ FK_AiInferenceRequests_Users is MISSING';
    SET @missingFKs = @missingFKs + 1;
END
ELSE
    PRINT '  ✓ FK_AiInferenceRequests_Users exists';

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AiInferenceRequests_ModelVersions')
BEGIN
    PRINT '  ✗ FK_AiInferenceRequests_ModelVersions is MISSING';
    SET @missingFKs = @missingFKs + 1;
END
ELSE
    PRINT '  ✓ FK_AiInferenceRequests_ModelVersions exists';

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AiDetectedRegions_InferenceRequests')
BEGIN
    PRINT '  ✗ FK_AiDetectedRegions_InferenceRequests is MISSING';
    SET @missingFKs = @missingFKs + 1;
END
ELSE
    PRINT '  ✓ FK_AiDetectedRegions_InferenceRequests exists';

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AiDetectedRegions_MangaPages')
BEGIN
    PRINT '  ✗ FK_AiDetectedRegions_MangaPages is MISSING';
    SET @missingFKs = @missingFKs + 1;
END
ELSE
    PRINT '  ✓ FK_AiDetectedRegions_MangaPages exists';

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AiDetectedRegions_PageRegions')
BEGIN
    PRINT '  ✗ FK_AiDetectedRegions_PageRegions is MISSING';
    SET @missingFKs = @missingFKs + 1;
END
ELSE
    PRINT '  ✓ FK_AiDetectedRegions_PageRegions exists';

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AiTaskSuggestions_MangaPages')
BEGIN
    PRINT '  ✗ FK_AiTaskSuggestions_MangaPages is MISSING';
    SET @missingFKs = @missingFKs + 1;
END
ELSE
    PRINT '  ✓ FK_AiTaskSuggestions_MangaPages exists';

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AiTaskSuggestions_PageRegions')
BEGIN
    PRINT '  ✗ FK_AiTaskSuggestions_PageRegions is MISSING';
    SET @missingFKs = @missingFKs + 1;
END
ELSE
    PRINT '  ✓ FK_AiTaskSuggestions_PageRegions exists';

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AiTaskSuggestions_Users')
BEGIN
    PRINT '  ✗ FK_AiTaskSuggestions_Users is MISSING';
    SET @missingFKs = @missingFKs + 1;
END
ELSE
    PRINT '  ✓ FK_AiTaskSuggestions_Users exists';

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AiEvaluationMetrics_Experiments')
BEGIN
    PRINT '  ✗ FK_AiEvaluationMetrics_Experiments is MISSING';
    SET @missingFKs = @missingFKs + 1;
END
ELSE
    PRINT '  ✓ FK_AiEvaluationMetrics_Experiments exists';

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AiEvaluationMetrics_ModelVersions')
BEGIN
    PRINT '  ✗ FK_AiEvaluationMetrics_ModelVersions is MISSING';
    SET @missingFKs = @missingFKs + 1;
END
ELSE
    PRINT '  ✓ FK_AiEvaluationMetrics_ModelVersions exists';

PRINT '';

-- Check Check Constraints
PRINT 'Checking Check Constraints...';
DECLARE @missingChecks INT = 0;

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_AiDetectedRegions_Confidence')
BEGIN
    PRINT '  ✗ CK_AiDetectedRegions_Confidence is MISSING';
    SET @missingChecks = @missingChecks + 1;
END
ELSE
    PRINT '  ✓ CK_AiDetectedRegions_Confidence exists';

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_AiDetectedRegions_Width')
BEGIN
    PRINT '  ✗ CK_AiDetectedRegions_Width is MISSING';
    SET @missingChecks = @missingChecks + 1;
END
ELSE
    PRINT '  ✓ CK_AiDetectedRegions_Width exists';

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_AiDetectedRegions_Height')
BEGIN
    PRINT '  ✗ CK_AiDetectedRegions_Height is MISSING';
    SET @missingChecks = @missingChecks + 1;
END
ELSE
    PRINT '  ✓ CK_AiDetectedRegions_Height exists';

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_AiTaskSuggestions_Status')
BEGIN
    PRINT '  ✗ CK_AiTaskSuggestions_Status is MISSING';
    SET @missingChecks = @missingChecks + 1;
END
ELSE
    PRINT '  ✓ CK_AiTaskSuggestions_Status exists';

PRINT '';

-- Check Indexes
PRINT 'Checking Indexes...';
DECLARE @missingIndexes INT = 0;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AiInferenceRequests_PageId_Status')
BEGIN
    PRINT '  ✗ IX_AiInferenceRequests_PageId_Status is MISSING';
    SET @missingIndexes = @missingIndexes + 1;
END
ELSE
    PRINT '  ✓ IX_AiInferenceRequests_PageId_Status exists';

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AiDetectedRegions_PageId')
BEGIN
    PRINT '  ✗ IX_AiDetectedRegions_PageId is MISSING';
    SET @missingIndexes = @missingIndexes + 1;
END
ELSE
    PRINT '  ✓ IX_AiDetectedRegions_PageId exists';

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AiTaskSuggestions_PageId_Status')
BEGIN
    PRINT '  ✗ IX_AiTaskSuggestions_PageId_Status is MISSING';
    SET @missingIndexes = @missingIndexes + 1;
END
ELSE
    PRINT '  ✓ IX_AiTaskSuggestions_PageId_Status exists';

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AiEvaluationMetrics_ModelVersionId')
BEGIN
    PRINT '  ✗ IX_AiEvaluationMetrics_ModelVersionId is MISSING';
    SET @missingIndexes = @missingIndexes + 1;
END
ELSE
    PRINT '  ✓ IX_AiEvaluationMetrics_ModelVersionId exists';

PRINT '';

-- Final Summary
PRINT '========================================';
PRINT 'Test Summary';
PRINT '========================================';
IF @missingTables = 0
    PRINT '✓ All 6 tables exist';
ELSE
    PRINT '✗ Missing ' + CAST(@missingTables AS VARCHAR) + ' tables';

IF @missingFKs = 0
    PRINT '✓ All 11 foreign key constraints exist';
ELSE
    PRINT '✗ Missing ' + CAST(@missingFKs AS VARCHAR) + ' foreign key constraints';

IF @missingChecks = 0
    PRINT '✓ All 4 check constraints exist';
ELSE
    PRINT '✗ Missing ' + CAST(@missingChecks AS VARCHAR) + ' check constraints';

IF @missingIndexes = 0
    PRINT '✓ All 4 indexes exist';
ELSE
    PRINT '✗ Missing ' + CAST(@missingIndexes AS VARCHAR) + ' indexes';

PRINT '';
IF @missingTables = 0 AND @missingFKs = 0 AND @missingChecks = 0 AND @missingIndexes = 0
BEGIN
    PRINT '========================================';
    PRINT '✓✓✓ ALL TESTS PASSED ✓✓✓';
    PRINT '========================================';
END
ELSE
BEGIN
    PRINT '========================================';
    PRINT '✗✗✗ SOME TESTS FAILED ✗✗✗';
    PRINT '========================================';
END
GO
