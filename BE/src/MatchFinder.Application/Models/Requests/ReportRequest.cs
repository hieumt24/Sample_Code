namespace MatchFinder.Application.Models.Requests
{
    public class GetListReportRequest : Pagination
    {
        public int? FieldId { get; set; }
        public string? Status { get; set; }
    }

    public class CreateReportRequest
    {
        public int FieldId { get; set; }
        public string Reason { get; set; }
        public string ReportType { get; set; }
    }

    public class UpdateReportRequest
    {
        public string Status { get; set; }
        public string AdminNotes { get; set; }
    }
}