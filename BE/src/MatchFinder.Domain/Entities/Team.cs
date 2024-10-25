using MatchFinder.Domain.Models;

namespace MatchFinder.Domain.Entities
{
    public class Team : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CaptainId { get; set; }
        public User Captain { get; set; }
        public ICollection<Booking> Bookings { get; set; }
        public ICollection<Fund> Funds { get; set; }
        public ICollection<TeamMember> TeamMembers { get; set; }
        public ICollection<TournamentRegistration> TournamentRegistrations { get; set; }
        public ICollection<MatchingTeam> MatchingTeams1 { get; set; }
        public ICollection<MatchingTeam> MatchingTeams2 { get; set; }
    }
}