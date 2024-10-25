namespace MatchFinder.Domain.Constants
{
    public static class OpponentFindingStatus
    {
        public static string FINDING = "FINDING";
        public static string ACCEPTED = "ACCEPTED";
        public static string CANCELLED = "CANCELLED";
        public static string OPPONENT_CANCELLED = "OPPONENT_CANCELLED";
        public static string OVERLAPPED_CANCELLED = "OVERLAPPED_CANCELLED";
    }

    public static class OpponentFindingRequestStatus
    {
        public static string CANCELLED = "CANCELLED";
        public static string SELF_CANCELLED = "SELF_CANCELLED";
        public static string OVERLAPPED_CANCELLED = "OVERLAPPED_CANCELLED";
    }
}