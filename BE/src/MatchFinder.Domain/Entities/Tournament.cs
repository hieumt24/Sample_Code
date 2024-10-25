using MatchFinder.Domain.Models;

namespace MatchFinder.Domain.Entities
{
    public class Tournament : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ICollection<TournamentRegistration> TournamentRegistrations { get; set; }
        public ICollection<MatchingTeam> MatchingTeams { get; set; }
    }
}