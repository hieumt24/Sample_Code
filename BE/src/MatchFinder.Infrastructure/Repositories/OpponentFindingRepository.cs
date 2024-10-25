using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Infrastructure.DataAccess;

namespace MatchFinder.Infrastructure.Repositories
{
    public class OpponentFindingRepository : GenericRepository<OpponentFinding>, IOpponentFindingRepository
    {
        public OpponentFindingRepository(MatchFinderContext context) : base(context)
        {
        }
    }
}