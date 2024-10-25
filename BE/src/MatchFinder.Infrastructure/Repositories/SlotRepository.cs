using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Infrastructure.DataAccess;

namespace MatchFinder.Infrastructure.Repositories
{
    public class SlotRepository : GenericRepository<Slot>, ISlotRepository
    {
        public SlotRepository(MatchFinderContext context) : base(context)
        {
        }
    }
}