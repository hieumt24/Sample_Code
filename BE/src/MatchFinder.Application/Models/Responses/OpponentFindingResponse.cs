namespace MatchFinder.Application.Models.Responses
{
    public class OpponentFindingResponse
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int UserFindingId { get; set; }
        public string UserFindingName { get; set; }
        public string UserFindingPhoneNumber { get; set; }
        public string UserFindingAvatar { get; set; }
        public int? FieldId { get; set; }
        public string? FieldName { get; set; }
        public string? Address { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Commune { get; set; }
        public int? BookingId { get; set; }
        public DateOnly? Date { get; set; }
        public String? StartTime { get; set; }
        public String? EndTime { get; set; }
        public string Status { get; set; }
        public int TotalRequest { get; set; }
        public bool IsOverdue { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public bool? IsCanRestore { get; set; } = null;
    }

    public class OpponentFindingResponseWithUserRequesting
    {
        public int Id { get; set; }
        public int UserRequestingId { get; set; }
        public string UserRequestingName { get; set; }
        public string UserRequestingPhoneNumber { get; set; }
        public string UserRequestingAvatar { get; set; }
        public int OpponentFindingId { get; set; }
        public string Message { get; set; }
        public bool IsAccepted { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public bool? IsCanRestore { get; set; } = null;
    }
}