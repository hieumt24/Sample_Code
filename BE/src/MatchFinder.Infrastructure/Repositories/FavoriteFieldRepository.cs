using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Infrastructure.DataAccess;

namespace MatchFinder.Infrastructure.Repositories
{
    public class FavoriteFieldRepository : GenericRepository<FavoriteField>, IFavoriteFieldRepository
    {
        public FavoriteFieldRepository(MatchFinderContext context) : base(context)
        {
        }
    }
}