using MatchFinder.Domain.Models;

namespace MatchFinder.Domain.Interfaces
{
    public interface INotificationHub
    {
        Task ReceiveNotification(NotificationResponse notification);
    }
}