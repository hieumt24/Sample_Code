using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Infrastructure.DataAccess;

namespace MatchFinder.Infrastructure.Repositories
{
    public class MenuRepository : GenericRepository<Menu>, IMenuRepository
    {
        public MenuRepository(MatchFinderContext context) : base(context)
        {
        }
    }
}