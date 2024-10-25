namespace MatchFinder.Application.Constants
{
    public class TransactionStatus
    {
        public const string PENDING = "PENDING";
        public const string FAILED = "FAILED";
        public const string SUCCESSFUL = "SUCCESSFUL";
        public const string CANCELLED = "CANCELLED";
    }

    public class TransactionType
    {
        public const string BOOKING = "BOOKING";
        public const string RECHARGE = "RECHARGE";
        public const string DEBTPAYMENT = "DEBTPAYMENT";
        public const string REFUND = "REFUND";
    }
}