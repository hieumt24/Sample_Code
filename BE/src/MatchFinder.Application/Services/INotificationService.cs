using MatchFinder.Application.Models.Requests;
using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Models;

namespace MatchFinder.Application.Services
{
    public interface INotificationService
    {
        Task SendNotificationToListUser(List<int> receiverIds, Notification notification);

        Task<RepositoryPaginationResponse<NotificationResponse>> GetUserNotifications(int userId, Pagination pagination);

        Task MarkNotificationAsRead(int notificationId, int userId);

        Task MarkAllNotificationAsRead(int userId);

        Task<int> CountUnReadNotificationByUserId(int userId);
    }
}