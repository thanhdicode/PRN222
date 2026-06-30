-- test_v4_constraints.sql
-- Script to validate that foreign key and check constraints are enforced
-- This tests referential integrity and data validation

USE MangaWorkflowDB;
GO

PRINT '========================================';
PRINT 'Testing MangaWorkflowDB_v4_ai_extension.sql Constraints';
PRINT '========================================';
PRINT '';

DECLARE @testsPassed INT = 0;
DECLARE @testsFailed INT = 0;

-- Test 1: Try to insert AiInferenceRequest with invalid PageId (should fail)
PRINT 'Test 1: Foreign Key - AiInferenceRequests.PageId → MangaPages';
BEGIN TRY
    INSERT INTO dbo.AiInferenceRequests (PageId, RequestedByUserId, RequestType, Status)
    VALUES (NEWID(), (SELECT TOP 1 UserId FROM dbo.Users), 'Segmentation', 'Pending');
    
    PRINT '  ✗ FAILED - Should have rejected invalid PageId';
    SET @testsFailed = @testsFailed + 1;
    ROLLBACK;
END TRY
BEGIN CATCH
    PRINT '  ✓ PASSED - Invalid PageId was rejected';
    SET @testsPassed = @testsPassed + 1;
END CATCH

-- Test 2: Try to insert AiInferenceRequest with invalid UserId (should fail)
PRINT 'Test 2: Foreign Key - AiInferenceRequests.RequestedByUserId → Users';
BEGIN TRY
    INSERT INTO dbo.AiInferenceRequests (PageId, RequestedByUserId, RequestType, Status)
    VALUES ((SELECT TOP 1 PageId FROM dbo.MangaPages), NEWID(), 'Segmentation', 'Pending');
    
    PRINT '  ✗ FAILED - Should have rejected invalid UserId';
    SET @testsFailed = @testsFailed + 1;
    ROLLBACK;
END TRY
BEGIN CATCH
    PRINT '  ✓ PASSED - Invalid UserId was rejected';
    SET @testsPassed = @testsPassed + 1;
END CATCH

-- Test 3: Try to insert AiDetectedRegion with invalid InferenceRequestId (should fail)
PRINT 'Test 3: Foreign Key - AiDetectedRegions.InferenceRequestId → AiInferenceRequests';
BEGIN TRY
    INSERT INTO dbo.AiDetectedRegions (InferenceRequestId, PageId, RegionTypeCode, X, Y, Width, Height, Confidence)
    VALUES (NEWID(), (SELECT TOP 1 PageId FROM dbo.MangaPages), 'Panel', 0, 0, 100, 100, 0.95);
    
    PRINT '  ✗ FAILED - Should have rejected invalid InferenceRequestId';
    SET @testsFailed = @testsFailed + 1;
    ROLLBACK;
END TRY
BEGIN CATCH
    PRINT '  ✓ PASSED - Invalid InferenceRequestId was rejected';
    SET @testsPassed = @testsPassed + 1;
END CATCH

-- Test 4: Try to insert AiDetectedRegion with Confidence > 1 (should fail)
PRINT 'Test 4: Check Constraint - AiDetectedRegions.Confidence (0-1)';
DECLARE @testInferenceId UNIQUEIDENTIFIER = NEWID();
DECLARE @testPageId UNIQUEIDENTIFIER = (SELECT TOP 1 PageId FROM dbo.MangaPages);
DECLARE @testUserId UNIQUEIDENTIFIER = (SELECT TOP 1 UserId FROM dbo.Users);

BEGIN TRY
    -- First create a valid inference request
    INSERT INTO dbo.AiInferenceRequests (InferenceRequestId, PageId, RequestedByUserId, RequestType, Status)
    VALUES (@testInferenceId, @testPageId, @testUserId, 'Segmentation', 'Pending');
    
    -- Try to insert region with invalid confidence
    INSERT INTO dbo.AiDetectedRegions (InferenceRequestId, PageId, RegionTypeCode, X, Y, Width, Height, Confidence)
    VALUES (@testInferenceId, @testPageId, 'Panel', 0, 0, 100, 100, 1.5);
    
    PRINT '  ✗ FAILED - Should have rejected Confidence > 1';
    SET @testsFailed = @testsFailed + 1;
    ROLLBACK;
END TRY
BEGIN CATCH
    PRINT '  ✓ PASSED - Confidence > 1 was rejected';
    SET @testsPassed = @testsPassed + 1;
    ROLLBACK;
END CATCH

-- Test 5: Try to insert AiDetectedRegion with Confidence < 0 (should fail)
PRINT 'Test 5: Check Constraint - AiDetectedRegions.Confidence (>= 0)';
SET @testInferenceId = NEWID();

BEGIN TRY
    -- First create a valid inference request
    INSERT INTO dbo.AiInferenceRequests (InferenceRequestId, PageId, RequestedByUserId, RequestType, Status)
    VALUES (@testInferenceId, @testPageId, @testUserId, 'Segmentation', 'Pending');
    
    -- Try to insert region with negative confidence
    INSERT INTO dbo.AiDetectedRegions (InferenceRequestId, PageId, RegionTypeCode, X, Y, Width, Height, Confidence)
    VALUES (@testInferenceId, @testPageId, 'Panel', 0, 0, 100, 100, -0.5);
    
    PRINT '  ✗ FAILED - Should have rejected Confidence < 0';
    SET @testsFailed = @testsFailed + 1;
    ROLLBACK;
END TRY
BEGIN CATCH
    PRINT '  ✓ PASSED - Confidence < 0 was rejected';
    SET @testsPassed = @testsPassed + 1;
    ROLLBACK;
END CATCH

-- Test 6: Try to insert AiDetectedRegion with Width <= 0 (should fail)
PRINT 'Test 6: Check Constraint - AiDetectedRegions.Width (> 0)';
SET @testInferenceId = NEWID();

BEGIN TRY
    -- First create a valid inference request
    INSERT INTO dbo.AiInferenceRequests (InferenceRequestId, PageId, RequestedByUserId, RequestType, Status)
    VALUES (@testInferenceId, @testPageId, @testUserId, 'Segmentation', 'Pending');
    
    -- Try to insert region with zero width
    INSERT INTO dbo.AiDetectedRegions (InferenceRequestId, PageId, RegionTypeCode, X, Y, Width, Height, Confidence)
    VALUES (@testInferenceId, @testPageId, 'Panel', 0, 0, 0, 100, 0.95);
    
    PRINT '  ✗ FAILED - Should have rejected Width <= 0';
    SET @testsFailed = @testsFailed + 1;
    ROLLBACK;
END TRY
BEGIN CATCH
    PRINT '  ✓ PASSED - Width <= 0 was rejected';
    SET @testsPassed = @testsPassed + 1;
    ROLLBACK;
END CATCH

-- Test 7: Try to insert AiDetectedRegion with Height <= 0 (should fail)
PRINT 'Test 7: Check Constraint - AiDetectedRegions.Height (> 0)';
SET @testInferenceId = NEWID();

BEGIN TRY
    -- First create a valid inference request
    INSERT INTO dbo.AiInferenceRequests (InferenceRequestId, PageId, RequestedByUserId, RequestType, Status)
    VALUES (@testInferenceId, @testPageId, @testUserId, 'Segmentation', 'Pending');
    
    -- Try to insert region with negative height
    INSERT INTO dbo.AiDetectedRegions (InferenceRequestId, PageId, RegionTypeCode, X, Y, Width, Height, Confidence)
    VALUES (@testInferenceId, @testPageId, 'Panel', 0, 0, 100, -50, 0.95);
    
    PRINT '  ✗ FAILED - Should have rejected Height <= 0';
    SET @testsFailed = @testsFailed + 1;
    ROLLBACK;
END TRY
BEGIN CATCH
    PRINT '  ✓ PASSED - Height <= 0 was rejected';
    SET @testsPassed = @testsPassed + 1;
    ROLLBACK;
END CATCH

-- Test 8: Try to insert AiTaskSuggestion with invalid Status (should fail)
PRINT 'Test 8: Check Constraint - AiTaskSuggestions.Status (Draft/Approved/Rejected)';
BEGIN TRY
    INSERT INTO dbo.AiTaskSuggestions (PageId, TaskTypeCode, Title, Status)
    VALUES (@testPageId, 'Other', 'Test Task', 'InvalidStatus');
    
    PRINT '  ✗ FAILED - Should have rejected invalid Status';
    SET @testsFailed = @testsFailed + 1;
    ROLLBACK;
END TRY
BEGIN CATCH
    PRINT '  ✓ PASSED - Invalid Status was rejected';
    SET @testsPassed = @testsPassed + 1;
END CATCH

-- Test 9: Insert valid AiDetectedRegion (should succeed)
PRINT 'Test 9: Valid AiDetectedRegion insertion';
SET @testInferenceId = NEWID();

BEGIN TRY
    BEGIN TRANSACTION;
    
    -- Create valid inference request
    INSERT INTO dbo.AiInferenceRequests (InferenceRequestId, PageId, RequestedByUserId, RequestType, Status)
    VALUES (@testInferenceId, @testPageId, @testUserId, 'Segmentation', 'Pending');
    
    -- Insert valid detected region
    INSERT INTO dbo.AiDetectedRegions (InferenceRequestId, PageId, RegionTypeCode, X, Y, Width, Height, Confidence)
    VALUES (@testInferenceId, @testPageId, 'Panel', 10, 20, 100, 150, 0.95);
    
    PRINT '  ✓ PASSED - Valid region was inserted';
    SET @testsPassed = @testsPassed + 1;
    
    ROLLBACK;
END TRY
BEGIN CATCH
    PRINT '  ✗ FAILED - Valid region should have been accepted';
    PRINT '  Error: ' + ERROR_MESSAGE();
    SET @testsFailed = @testsFailed + 1;
    ROLLBACK;
END CATCH

-- Test 10: Insert valid AiTaskSuggestion with Draft status (should succeed)
PRINT 'Test 10: Valid AiTaskSuggestion insertion';
BEGIN TRY
    BEGIN TRANSACTION;
    
    INSERT INTO dbo.AiTaskSuggestions (PageId, TaskTypeCode, Title, Status)
    VALUES (@testPageId, 'Other', 'Test Task', 'Draft');
    
    PRINT '  ✓ PASSED - Valid task suggestion was inserted';
    SET @testsPassed = @testsPassed + 1;
    
    ROLLBACK;
END TRY
BEGIN CATCH
    PRINT '  ✗ FAILED - Valid suggestion should have been accepted';
    PRINT '  Error: ' + ERROR_MESSAGE();
    SET @testsFailed = @testsFailed + 1;
    ROLLBACK;
END CATCH

-- Final Summary
PRINT '';
PRINT '========================================';
PRINT 'Test Summary';
PRINT '========================================';
PRINT 'Tests Passed: ' + CAST(@testsPassed AS VARCHAR);
PRINT 'Tests Failed: ' + CAST(@testsFailed AS VARCHAR);
PRINT '';

IF @testsFailed = 0
BEGIN
    PRINT '========================================';
    PRINT '✓✓✓ ALL CONSTRAINT TESTS PASSED ✓✓✓';
    PRINT '========================================';
END
ELSE
BEGIN
    PRINT '========================================';
    PRINT '✗✗✗ SOME CONSTRAINT TESTS FAILED ✗✗✗';
    PRINT '========================================';
END
GO
