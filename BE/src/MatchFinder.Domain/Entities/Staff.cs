using MatchFinder.Domain.Models;

namespace MatchFinder.Domain.Entities
{
    public class Staff : BaseEntity
    {
        public int FieldId { get; set; }
        public Field Field { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public bool IsActive { get; set; }
        public string Name { get; set; }
    }
}