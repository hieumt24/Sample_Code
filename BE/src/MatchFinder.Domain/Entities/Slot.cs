using MatchFinder.Domain.Models;

namespace MatchFinder.Domain.Entities
{
    public class Slot : BaseEntity
    {
        public int Id { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public int FieldId { get; set; }
        public Field Field { get; set; }
    }
}