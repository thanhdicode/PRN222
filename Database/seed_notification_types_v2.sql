-- Idempotent SET 1 notification type seed script.
-- Run against the MangaWorkflowDB database after the base demo scripts.

SET NOCOUNT ON;

INSERT INTO dbo.NotificationTypes (TypeCode, TypeName)
SELECT 'UserLogin', N'User Login'
WHERE NOT EXISTS (SELECT 1 FROM dbo.NotificationTypes WHERE TypeCode = 'UserLogin');

INSERT INTO dbo.NotificationTypes (TypeCode, TypeName)
SELECT 'UserLogout', N'User Logout'
WHERE NOT EXISTS (SELECT 1 FROM dbo.NotificationTypes WHERE TypeCode = 'UserLogout');

INSERT INTO dbo.NotificationTypes (TypeCode, TypeName)
SELECT 'SeriesSubmitted', N'Series Submitted'
WHERE NOT EXISTS (SELECT 1 FROM dbo.NotificationTypes WHERE TypeCode = 'SeriesSubmitted');

INSERT INTO dbo.NotificationTypes (TypeCode, TypeName)
SELECT 'SubmissionUploaded', N'Submission Uploaded'
WHERE NOT EXISTS (SELECT 1 FROM dbo.NotificationTypes WHERE TypeCode = 'SubmissionUploaded');

INSERT INTO dbo.NotificationTypes (TypeCode, TypeName)
SELECT 'SubmissionReviewed', N'Submission Reviewed'
WHERE NOT EXISTS (SELECT 1 FROM dbo.NotificationTypes WHERE TypeCode = 'SubmissionReviewed');

INSERT INTO dbo.NotificationTypes (TypeCode, TypeName)
SELECT 'System', N'System Notification'
WHERE NOT EXISTS (SELECT 1 FROM dbo.NotificationTypes WHERE TypeCode = 'System');

SELECT TypeCode, TypeName
FROM dbo.NotificationTypes
WHERE TypeCode IN (
    'UserLogin',
    'UserLogout',
    'SeriesSubmitted',
    'SubmissionUploaded',
    'SubmissionReviewed',
    'System'
)
ORDER BY TypeCode;
