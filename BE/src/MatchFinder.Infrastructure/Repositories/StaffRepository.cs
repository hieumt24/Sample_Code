using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Infrastructure.DataAccess;

namespace MatchFinder.Infrastructure.Repositories
{
    public class StaffRepository : GenericRepository<Staff>, IStaffRepository
    {
        public StaffRepository(MatchFinderContext context) : base(context)
        {
        }
    }
}