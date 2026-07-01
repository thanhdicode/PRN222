namespace MangaWorkflow.Domain.Constants
{
    public static class NotificationTypeCodes
    {
        // Board review workflow
        public const string BoardVoteCast    = "BoardVoteCast";
        public const string SeriesApproved   = "SeriesApproved";
        public const string SeriesRejected   = "SeriesRejected";
        public const string SeriesSubmitted  = "SeriesSubmitted";

        // Ranking
        public const string RankingUpdated   = "RankingUpdated";

        // Task workflow
        public const string TaskAssigned     = "TaskAssigned";
        public const string TaskApproved     = "TaskApproved";
        public const string TaskRejected     = "TaskRejected";
        public const string DeadlineReminder = "DeadlineReminder";

        // Submission workflow
        public const string SubmissionUploaded  = "SubmissionUploaded";
        public const string SubmissionReviewed  = "SubmissionReviewed";

        // Auth audit
        public const string UserLogin  = "UserLogin";
        public const string UserLogout = "UserLogout";

        // System
        public const string System = "System";
    }
}
