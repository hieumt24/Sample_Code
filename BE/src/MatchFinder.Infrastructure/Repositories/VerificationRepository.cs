using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Infrastructure.DataAccess;

namespace MatchFinder.Infrastructure.Repositories
{
    internal class VerificationRepository : GenericRepository<Verification>, IVerificationRepository
    {
        public VerificationRepository(MatchFinderContext context) : base(context)
        {
        }
    }
}