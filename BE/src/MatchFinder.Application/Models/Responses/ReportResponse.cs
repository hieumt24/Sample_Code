namespace MatchFinder.Application.Models.Responses
{
    public class ReportResponse
    {
        public int Id { get; set; }
        public int FieldId { get; set; }
        public int UserCreatedId { get; set; }
        public string UserCreated { get; set; }
        public string Reason { get; set; }
        public string ReportType { get; set; }
        public string Status { get; set; }
        public string AdminNotes { get; set; }
    }
}