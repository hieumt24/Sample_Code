namespace MatchFinder.Application.Models.Responses
{
    public class BookingResponse
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public String StartTime { get; set; }
        public String EndTime { get; set; }
        public string Status { get; set; }
        public decimal DepositAmount { get; set; }
        public int PartialFieldId { get; set; }
        public string PartialFieldName { get; set; }
        public int UserId { get; set; }
        public string UserBookingName { get; set; }
        public string UserBookingPhoneNumber { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public string PaymentLink { get; set; }
        public int FieldId { get; set; }
        public string FieldName { get; set; }
        public string FieldAvatar { get; set; }
        public string FieldStar { get; set; }
        public bool IsRated { get; set; }
        public int? PostId { get; set; }
        public int OwnerId { get; set; }
        public string OwnerName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
    }
}