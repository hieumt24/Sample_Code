namespace MatchFinder.Application.Models.Responses
{
    public class InactiveTimeResponse
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Reason { get; set; }
        public int FieldId { get; set; }
    }
}