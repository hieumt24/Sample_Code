using MatchFinder.Domain.Models;

namespace MatchFinder.Domain.Entities
{
    public class Fund : BaseEntity
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public int TeamId { get; set; }
        public Team Team { get; set; }
    }
}