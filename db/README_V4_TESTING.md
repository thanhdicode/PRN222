# MangaWorkflowDB v4 AI Extension Testing

This document describes the testing approach for validating the `MangaWorkflowDB_v4_ai_extension.sql` script integrity and constraints.

## Overview

Task 6 of the AI Studio V1 Stabilization Specification requires adding foreign key and check constraints to the DB v4 AI extension script to ensure referential integrity and data validation.

## Implementation Summary

The `MangaWorkflowDB_v4_ai_extension.sql` script has been enhanced with:

### 1. **Consistent Schema Prefix**
- All table references use `dbo.` schema prefix (36 occurrences)
- Ensures proper object resolution and prevents naming conflicts

### 2. **Foreign Key Constraints (11 total)**

#### AiInferenceRequests (3 FKs)
- `FK_AiInferenceRequests_MangaPages`: PageId → MangaPages(PageId)
- `FK_AiInferenceRequests_Users`: RequestedByUserId → Users(UserId)
- `FK_AiInferenceRequests_ModelVersions`: ModelVersionId → AiModelVersions(ModelVersionId)

#### AiDetectedRegions (3 FKs)
- `FK_AiDetectedRegions_InferenceRequests`: InferenceRequestId → AiInferenceRequests(InferenceRequestId)
- `FK_AiDetectedRegions_MangaPages`: PageId → MangaPages(PageId)
- `FK_AiDetectedRegions_PageRegions`: PageRegionId → PageRegions(RegionId)

#### AiTaskSuggestions (3 FKs)
- `FK_AiTaskSuggestions_MangaPages`: PageId → MangaPages(PageId)
- `FK_AiTaskSuggestions_PageRegions`: PageRegionId → PageRegions(RegionId)
- `FK_AiTaskSuggestions_Users`: SuggestedAssistantId → Users(UserId)

#### AiEvaluationMetrics (2 FKs)
- `FK_AiEvaluationMetrics_Experiments`: ExperimentId → AiTrainingExperiments(ExperimentId)
- `FK_AiEvaluationMetrics_ModelVersions`: ModelVersionId → AiModelVersions(ModelVersionId)

### 3. **Check Constraints (4 total)**

#### AiDetectedRegions (3 checks)
- `CK_AiDetectedRegions_Confidence`: Ensures Confidence is between 0 and 1
- `CK_AiDetectedRegions_Width`: Ensures Width > 0
- `CK_AiDetectedRegions_Height`: Ensures Height > 0

#### AiTaskSuggestions (1 check)
- `CK_AiTaskSuggestions_Status`: Ensures Status is one of: 'Draft', 'Approved', 'Rejected'

### 4. **Idempotency Guards**
- All constraints use `IF NOT EXISTS` guards (21 total)
- Script can be run multiple times safely without errors
- Prevents duplicate constraint creation attempts

## Testing Approach

Three test scripts have been created to validate the implementation:

### Test 1: Integrity Test (`test_v4_integrity.sql`)

**Purpose**: Verify all database objects exist after running the v4 script

**Validates**:
- ✓ 6 tables exist (AiModelVersions, AiInferenceRequests, AiDetectedRegions, AiTaskSuggestions, AiTrainingExperiments, AiEvaluationMetrics)
- ✓ 11 foreign key constraints exist
- ✓ 4 check constraints exist
- ✓ 4 indexes exist

**How to Run**:
```sql
sqlcmd -S localhost -d MangaWorkflowDB -E -i test_v4_integrity.sql
```

**Expected Output**: "ALL TESTS PASSED"

---

### Test 2: Idempotency Test (`test_v4_idempotency.sql`)

**Purpose**: Verify the script can be run multiple times without errors

**Validates**:
- Script runs successfully on first execution
- Script runs successfully on second execution
- No duplicate constraint errors occur
- IF NOT EXISTS guards work correctly

**How to Run**:
```sql
sqlcmd -S localhost -d MangaWorkflowDB -E -i test_v4_idempotency.sql
```

**Expected Output**: "IDEMPOTENCY TEST PASSED"

---

### Test 3: Constraint Enforcement Test (`test_v4_constraints.sql`)

**Purpose**: Verify foreign key and check constraints are actually enforced

**Validates**:

#### Foreign Key Tests (should reject invalid references)
1. ✗ Invalid PageId in AiInferenceRequests
2. ✗ Invalid UserId in AiInferenceRequests
3. ✗ Invalid InferenceRequestId in AiDetectedRegions

#### Check Constraint Tests (should reject invalid values)
4. ✗ Confidence > 1 in AiDetectedRegions
5. ✗ Confidence < 0 in AiDetectedRegions
6. ✗ Width <= 0 in AiDetectedRegions
7. ✗ Height <= 0 in AiDetectedRegions
8. ✗ Invalid Status in AiTaskSuggestions

#### Valid Data Tests (should succeed)
9. ✓ Valid AiDetectedRegion insertion
10. ✓ Valid AiTaskSuggestion insertion

**How to Run**:
```sql
sqlcmd -S localhost -d MangaWorkflowDB -E -i test_v4_constraints.sql
```

**Expected Output**: "ALL CONSTRAINT TESTS PASSED" (10/10 tests passed)

---

## Running All Tests

To run the complete test suite:

```powershell
# Run integrity test
sqlcmd -S localhost -d MangaWorkflowDB -E -i test_v4_integrity.sql

# Run idempotency test
sqlcmd -S localhost -d MangaWorkflowDB -E -i test_v4_idempotency.sql

# Run constraint enforcement test
sqlcmd -S localhost -d MangaWorkflowDB -E -i test_v4_constraints.sql
```

## Acceptance Criteria Verification

✅ **Script runs without errors**: All constraints use IF NOT EXISTS guards  
✅ **All constraints enforced**: Foreign keys and check constraints validated by test_v4_constraints.sql  
✅ **Can run multiple times safely**: Verified by test_v4_idempotency.sql  
✅ **Consistent dbo. prefix**: 36 references throughout the script  
✅ **Referential integrity**: 11 foreign key constraints covering all relationships  
✅ **Data validation**: 4 check constraints ensure data quality  

## Notes

- Tests use ROLLBACK to avoid polluting the database with test data
- All tests are non-destructive and safe to run on production databases
- Tests require existing data in MangaPages and Users tables (from v2/v3 scripts)
- If base tables don't exist, run `MangaWorkflowDB_v2_demo_ready.sql` first

## Integration with DbSmokeTest

After running these validation tests, run the project's existing DbSmokeTest to ensure:
- Application can connect to the database
- Entity Framework mappings work correctly
- No breaking changes to existing features

```bash
dotnet test --filter "FullyQualifiedName~DbSmokeTest"
```
