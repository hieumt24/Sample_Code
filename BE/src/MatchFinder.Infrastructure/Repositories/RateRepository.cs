using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Infrastructure.DataAccess;

namespace MatchFinder.Infrastructure.Repositories
{
    public class RateRepository : GenericRepository<Rate>, IRateRepository
    {
        public RateRepository(MatchFinderContext context) : base(context)
        {
        }
    }
}