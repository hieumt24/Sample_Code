using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Infrastructure.DataAccess;

namespace MatchFinder.Infrastructure.Repositories
{
    public class ReportRepository : GenericRepository<Report>, IReportRepository
    {
        public ReportRepository(MatchFinderContext context) : base(context)
        {
        }
    }
}