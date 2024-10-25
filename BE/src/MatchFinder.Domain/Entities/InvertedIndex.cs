namespace MatchFinder.Domain.Entities
{
    public class InvertedIndex
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public int FieldId { get; set; }
        public int RecordId { get; set; }
    }
}