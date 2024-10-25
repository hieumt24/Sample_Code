using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace MatchFinder.Infrastructure.Repositories
{
    public class OpponentFindingRequestRepository : GenericRepository<OpponentFindingRequest>, IOpponentFindingRequestRepository
    {
        public OpponentFindingRequestRepository(MatchFinderContext context) : base(context)
        {
        }

        public async Task<IEnumerable<OpponentFindingRequest>> GetListUserRequestByOpponentFindingId(int opponentFindingId, int offset, int limit, bool IsSortDescByCreatedAt)
        {
            var query = _context.OpponentFindingRequests
                .Where(x => x.OpponentFindingId == opponentFindingId)
                .Include(x => x.UserRequesting)
                .Include(x => x.OpponentFinding)
                .OrderByDescending(x => x.IsAccepted)
                .ThenBy(x => x.Status);

            if (IsSortDescByCreatedAt)
            {
                query = query.ThenByDescending(x => x.CreatedAt);
            }
            else
            {
                query = query.ThenBy(x => x.CreatedAt);
            }

            return await query
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }
    }
}