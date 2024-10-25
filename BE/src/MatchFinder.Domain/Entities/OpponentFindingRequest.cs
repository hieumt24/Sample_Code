using MatchFinder.Domain.Models;

namespace MatchFinder.Domain.Entities
{
    public class OpponentFindingRequest : BaseEntity
    {
        public int Id { get; set; }
        public int UserRequestingId { get; set; }
        public User UserRequesting { get; set; }
        public int OpponentFindingId { get; set; }
        public OpponentFinding OpponentFinding { get; set; }
        public string? Message { get; set; }
        public bool IsAccepted { get; set; } = false;
        public string? Status { get; set; }
    }
}