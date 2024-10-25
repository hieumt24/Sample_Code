using MatchFinder.Domain.Models;

namespace MatchFinder.Domain.Entities
{
    public class Image : BaseEntity
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string? Content { get; set; }
        public int FieldId { get; set; }
        public Field Field { get; set; }
    }
}