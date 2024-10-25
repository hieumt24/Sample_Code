using MatchFinder.Domain.Models;

namespace MatchFinder.Domain.Entities
{
    public class TournamentRegistration : BaseEntity
    {
        public int TournamentId { get; set; }
        public Tournament Tournament { get; set; }
        public int TeamId { get; set; }
        public Team Team { get; set; }
    }
}