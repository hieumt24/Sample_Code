using MatchFinder.Domain.Models;

namespace MatchFinder.Domain.Entities
{
    public class PartialField : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public int FieldId { get; set; }
        public Field Field { get; set; }
        public ICollection<Booking> Bookings { get; set; }
        public ICollection<FieldRevenue> FieldRevenues { get; set; }
        public ICollection<MatchingTeam> MatchingTeams { get; set; }
    }
}