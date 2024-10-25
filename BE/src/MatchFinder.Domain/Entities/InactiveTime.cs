using MatchFinder.Domain.Models;

namespace MatchFinder.Domain.Entities
{
    public class InactiveTime : BaseEntity
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Reason { get; set; }
        public int FieldId { get; set; }
        public Field Field { get; set; }
    }
}