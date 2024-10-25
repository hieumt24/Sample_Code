namespace MatchFinder.Application.Models.Responses
{
    public class StatisticBookingMonthlyResponse
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public int Total { get; set; }

        // Parameterized constructor
        public StatisticBookingMonthlyResponse(int month, int year)
        {
            Month = month;
            Year = year;
            Total = 0; // Default total to 0
        }
    }

    public class StatisticBookingWeekDayResponse
    {
        public int Monday { get; set; }
        public int Tuesday { get; set; }
        public int Wednesday { get; set; }
        public int Thursday { get; set; }
        public int Friday { get; set; }
        public int Saturday { get; set; }
        public int Sunday { get; set; }
    }

    public class StatisticBookingStatusResponse
    {
        public int CanceledTotal { get; set; }
        public int AcceptedTotal { get; set; }
        public int RejectedTotal { get; set; }
        public List<StatusMonthly> Monthly { get; set; } = new List<StatusMonthly>();
    }

    public class StatusMonthly
    {
        public int Canceled { get; set; }
        public int Accepted { get; set; }
        public int Rejected { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        // Parameterized constructor
        public StatusMonthly(int month, int year)
        {
            Month = month;
            Year = year;
            Canceled = 0;
            Accepted = 0;
            Rejected = 0;
        }
    }

    public class StatisticRegisterMonthlyResponse
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public int Total { get; set; }

        // Parameterized constructor
        public StatisticRegisterMonthlyResponse(int month, int year)
        {
            Month = month;
            Year = year;
            Total = 0; // Default total to 0
        }
    }
}