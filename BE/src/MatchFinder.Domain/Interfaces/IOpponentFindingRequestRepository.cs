using MatchFinder.Domain.Entities;

namespace MatchFinder.Domain.Interfaces
{
    public interface IOpponentFindingRequestRepository : IGenericRepository<OpponentFindingRequest>
    {
        Task<IEnumerable<OpponentFindingRequest>> GetListUserRequestByOpponentFindingId(int opponentFindingId, int offset, int limit, bool IsSortDescByCreatedAt);
    }
}