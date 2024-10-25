namespace MatchFinder.Application.Models.Responses
{
    public class SlotResponse
    {
        public int Id { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int FieldId { get; set; }
        public string FieldName { get; set; }
    }
}