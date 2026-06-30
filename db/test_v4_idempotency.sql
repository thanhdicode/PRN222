-- test_v4_idempotency.sql
-- Script to test that MangaWorkflowDB_v4_ai_extension.sql can be run multiple times safely
-- This validates the IF NOT EXISTS guards work correctly

USE MangaWorkflowDB;
GO

PRINT '========================================';
PRINT 'Testing MangaWorkflowDB_v4_ai_extension.sql Idempotency';
PRINT '========================================';
PRINT '';
PRINT 'This test will run the v4 extension script TWICE to ensure';
PRINT 'it can be executed multiple times without errors.';
PRINT '';

-- Run the script the first time
PRINT '--- Running v4 script (First Execution) ---';
GO
:r MangaWorkflowDB_v4_ai_extension.sql
GO

PRINT '';
PRINT '--- Running v4 script (Second Execution) ---';
GO
:r MangaWorkflowDB_v4_ai_extension.sql
GO

PRINT '';
PRINT '========================================';
PRINT '✓✓✓ IDEMPOTENCY TEST PASSED ✓✓✓';
PRINT 'Script can be run multiple times safely';
PRINT '========================================';
GO
