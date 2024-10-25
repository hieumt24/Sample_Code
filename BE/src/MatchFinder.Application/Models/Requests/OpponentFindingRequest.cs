using MatchFinder.Application.Attributes;
using System.ComponentModel.DataAnnotations;

namespace MatchFinder.Application.Models.Requests
{
    public class OpponentFindingCreateRequest
    {
        public string Content { get; set; } = string.Empty;
        public int BookingId { get; set; }
    }

    public class OpponentFindingNotBookingCreateRequest
    {
        [Required]
        public string Content { get; set; } = string.Empty;

        public string? FieldName { get; set; }
        public string? FieldAddress { get; set; }

        [Required]
        public string FieldProvince { get; set; }

        [Required]
        public string FieldDistrict { get; set; }

        public string? FieldCommune { get; set; }

        [Required]
        [Range(0, 86400, ErrorMessage = "StartTime must be between 0h and 24h")]
        public int StartTime { get; set; }

        [Required]
        [Range(0, 86400, ErrorMessage = "EndTime must be between 0h and 24h")]
        [GreaterThanOrEqualTo("StartTime", ErrorMessage = "EndTime must greater than or equal to StartTime")]
        public int EndTime { get; set; }

        [Required]
        public DateOnly Date { get; set; }
    }

    public class OpponentFindingUpdateRequest
    {
        public string? Content { get; set; }
    }

    public class OpponentFindingFilterRequest : Pagination
    {
        public string? FieldName { get; set; } = string.Empty;
        public string? Province { get; set; } = string.Empty;
        public string? District { get; set; } = string.Empty;
        public string? Commune { get; set; } = string.Empty;
        public string? Address { get; set; } = string.Empty;
        public DateOnly? FromDate { get; set; }

        [GreaterThanOrEqualTo("FromDate", ErrorMessage = "ToDate must greater than or equal to FromDate")]
        public DateOnly? ToDate { get; set; }

        [Range(0, 86400, ErrorMessage = "FromTime must be between 0h and 24h")]
        public int? FromTime { get; set; }

        [Range(0, 86400, ErrorMessage = "ToTime must be between 0h and 24h")]
        [GreaterThanOrEqualTo("FromTime", ErrorMessage = "ToTime must greater than or equal to FromTime")]
        public int? ToTime { get; set; }

        [RegularExpression("FINDING|ACCEPTED|CANCELLED|OPPONENT_CANCELLED|OVERLAPPED_CANCELLED", ErrorMessage = "Status must be FINDING, ACCEPTED, CANCELLED")]
        public string? Status { get; set; } = string.Empty;
    }

    public class RequestingOpponentFindingRequest
    {
        public int OpponentFindingId { get; set; }

        [Required]
        public string Message { get; set; } = string.Empty;
    }

    public class RequestingOpponentFindingFilter : Pagination
    {
        public bool IsSortDescByCreatedAt { get; set; } = true;
    }
}