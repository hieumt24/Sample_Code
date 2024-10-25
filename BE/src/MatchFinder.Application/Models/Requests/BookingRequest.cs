using MatchFinder.Application.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MatchFinder.Application.Models.Requests
{
    public class BookingCreateRequest
    {
        public DateOnly Date { get; set; }

        [Range(0, 86400, ErrorMessage = "OpenTime must be between 0h and 24h00")]
        public int StartTime { get; set; }

        [Range(0, 86400, ErrorMessage = "OpenTime must be between 0h and 24h00")]
        [GreaterThanOrEqualTo("StartTime", ErrorMessage = "EndTime must greater than or equal to StartTime")]
        public int EndTime { get; set; }

        public int PartialFieldId { get; set; }
        public int TeamId { get; set; }
    }

    public class FieldAutoBookingRequest
    {
        public DateOnly Date { get; set; }

        [Range(0, 86400, ErrorMessage = "OpenTime must be between 0h and 24h00")]
        public int StartTime { get; set; }

        [Range(0, 86400, ErrorMessage = "OpenTime must be between 0h and 24h00")]
        [GreaterThanOrEqualTo("StartTime", ErrorMessage = "EndTime must greater than or equal to StartTime")]
        public int EndTime { get; set; }

        public int FieldId { get; set; }
    }

    public class BookingUpdateRequest
    {
        public int Id { get; set; }
        public DateOnly? Date { get; set; }

        [AllowNull]
        [Range(0, 86400, ErrorMessage = "OpenTime must be between 0h and 24h")]
        public int? StartTime { get; set; }

        [AllowNull]
        [Range(0, 86400, ErrorMessage = "OpenTime must be between 0h and 24h")]
        [GreaterThanOrEqualTo("StartTime", ErrorMessage = "EndTime must greater than or equal to StartTime")]
        public int? EndTime { get; set; }
    }

    public class SearchBookingRequest : Pagination
    {
        public DateOnly? Date { get; set; }

        [AllowNull]
        [Range(0, 86400, ErrorMessage = "OpenTime must be between 0h and 24h")]
        public int? StartTime { get; set; }

        [AllowNull]
        [Range(0, 86400, ErrorMessage = "OpenTime must be between 0h and 24h")]
        [GreaterThanOrEqualTo("StartTime", ErrorMessage = "EndTime must greater than or equal to StartTime")]
        public int? EndTime { get; set; }

        public int? UserId { get; set; }
        public int? PartialFieldId { get; set; }
        public string? Status { get; set; }
    }

    public class SearchBookingByFieldRequest : Pagination
    {
        public DateOnly? Date { get; set; }

        [AllowNull]
        [Range(0, 86400, ErrorMessage = "OpenTime must be between 0h and 24h")]
        public int? StartTime { get; set; }

        [AllowNull]
        [Range(0, 86400, ErrorMessage = "OpenTime must be between 0h and 24h")]
        [GreaterThanOrEqualTo("StartTime", ErrorMessage = "EndTime must greater than or equal to StartTime")]
        public int? EndTime { get; set; }

        public int? UserId { get; set; }
        public string? Status { get; set; }
    }

    public class MyBookingRequest : Pagination
    {
        [AllowNull]
        public DateOnly? Date { get; set; }

        [AllowNull]
        public DateOnly? StartTime { get; set; }

        [AllowNull]
        public DateOnly? EndTime { get; set; }

        public int? FieldId { get; set; }
        public string? Status { get; set; }
    }

    public class RejectionRequest
    {
        public int Id { get; set; }
    }

    public class HandleStatusRequest
    {
        public int Id { get; set; }
        public string Status { get; set; }
    }

    public class ListBookingBusyRequest
    {
        public DateOnly? Date { get; set; }

        [AllowNull]
        [Range(0, 86400, ErrorMessage = "OpenTime must be between 0h and 24h")]
        public int? StartTime { get; set; }

        [AllowNull]
        [Range(0, 86400, ErrorMessage = "OpenTime must be between 0h and 24h")]
        [GreaterThanOrEqualTo("StartTime", ErrorMessage = "EndTime must greater than or equal to StartTime")]
        public int? EndTime { get; set; }

        public DateOnly? StartDate { get; set; }

        [GreaterThanOrEqualTo("StartDate", ErrorMessage = "EndDate must greater than or equal to StartDate")]
        public DateOnly? EndDate { get; set; }

        public int? PartialFieldId { get; set; }
    }

    public class GetBookingByFieldAndDatesRequest
    {
        public int FieldId { get; set; }
        public DateOnly StartDate { get; set; }

        [GreaterThanOrEqualTo("StartDate", ErrorMessage = "Thời gian bắt đầu phải diễn ra trước thời gian kết thúc")]
        public DateOnly EndDate { get; set; }
    }
}