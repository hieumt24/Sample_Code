using MatchFinder.Domain.Models;

namespace MatchFinder.Domain.Entities
{
    public class MatchingTeam : BaseEntity
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public DateTime MatchDate { get; set; }
        public int Team1Id { get; set; }
        public Team Team1 { get; set; }
        public int Team2Id { get; set; }
        public Team Team2 { get; set; }
        public int PartialFieldId { get; set; }
        public PartialField PartialField { get; set; }
        public int TournamentId { get; set; }
        public Tournament Tournament { get; set; }
    }
}