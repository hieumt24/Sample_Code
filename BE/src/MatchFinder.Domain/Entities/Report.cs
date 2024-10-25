using MatchFinder.Domain.Models;

namespace MatchFinder.Domain.Entities
{
    public class Report : BaseEntity
    {
        public int Id { get; set; }
        public string Reason { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string AdminNotes { get; set; }
        public int FieldId { get; set; }
        public int UserId { get; set; }
        public virtual Field Field { get; set; }
        public virtual User Reporter { get; set; }
    }
}