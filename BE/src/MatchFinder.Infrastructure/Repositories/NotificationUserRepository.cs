using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Infrastructure.DataAccess;

namespace MatchFinder.Infrastructure.Repositories
{
    public class NotificationUserRepository : GenericRepository<NotificationUser>, INotificationUserRepository
    {
        public NotificationUserRepository(MatchFinderContext context) : base(context)
        {
        }
    }
}