using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Infrastructure.DataAccess;

namespace MatchFinder.Infrastructure.Repositories
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(MatchFinderContext context) : base(context)
        {
        }
    }
}