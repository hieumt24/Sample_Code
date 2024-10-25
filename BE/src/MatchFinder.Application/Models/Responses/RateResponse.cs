namespace MatchFinder.Application.Models.Responses
{
    public class RateResponse
    {
        public int Star { get; set; }
        public string Comment { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int UserId { get; set; }
        public string RaterName { get; set; }
        public string RaterAvatar { get; set; }
        public int BookingId { get; set; }
        public int FieldId { get; set; }
        public string FieldName { get; set; }
    }
}