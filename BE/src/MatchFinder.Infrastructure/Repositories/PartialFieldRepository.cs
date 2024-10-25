using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Infrastructure.DataAccess;

namespace MatchFinder.Infrastructure.Repositories
{
    public class PartialFieldRepository : GenericRepository<PartialField>, IPartialFieldRepository
    {
        public PartialFieldRepository(MatchFinderContext context) : base(context)
        {
        }
    }
}