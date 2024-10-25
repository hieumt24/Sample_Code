using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Infrastructure.DataAccess;

namespace MatchFinder.Infrastructure.Repositories
{
    public class TeamRepository : GenericRepository<Team>, ITeamRepository
    {
        public TeamRepository(MatchFinderContext context) : base(context)
        {
        }
    }
}