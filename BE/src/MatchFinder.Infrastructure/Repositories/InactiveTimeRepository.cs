using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Infrastructure.DataAccess;

namespace MatchFinder.Infrastructure.Repositories
{
    public class InactiveTimeRepository : GenericRepository<InactiveTime>, IInactiveTimeRepository
    {
        public InactiveTimeRepository(MatchFinderContext context) : base(context)
        {
        }
    }
}