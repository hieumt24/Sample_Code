using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Infrastructure.DataAccess;

namespace MatchFinder.Infrastructure.Repositories
{
    internal class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(MatchFinderContext context) : base(context)
        {
        }
    }
}