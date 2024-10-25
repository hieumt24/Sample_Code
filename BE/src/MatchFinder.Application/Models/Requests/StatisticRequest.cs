using MatchFinder.Application.Attributes;

namespace MatchFinder.Application.Models.Requests
{
    public class StatisticRequestFilter
    {
        public DateOnly FromDate { get; set; }

        [GreaterThanOrEqualTo("FromDate", ErrorMessage = "ToDate must greater than or equal to FromDate")]
        public DateOnly ToDate { get; set; }

        public int? FieldId { get; set; }
    }

    public class StatisticBookingSlotRequest
    {
        public DateOnly FromDate { get; set; }

        [GreaterThanOrEqualTo("FromDate", ErrorMessage = "ToDate must greater than or equal to FromDate")]
        public DateOnly ToDate { get; set; }

        public int FieldId { get; set; }
    }
}