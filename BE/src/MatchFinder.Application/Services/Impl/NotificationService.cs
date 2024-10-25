using AutoMapper;
using MatchFinder.Application.Models.Requests;
using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Exceptions;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Domain.Models;
using MatchFinder.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace MatchFinder.Application.Services.Impl
{
    public class NotificationService : INotificationService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<NotificationHub, INotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub, INotificationHub> hubContext, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _hubContext = hubContext;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<RepositoryPaginationResponse<NotificationResponse>> GetUserNotifications(int userId, Pagination pagination)
        {
            var notifications = await _unitOfWork.NotificationUserRepository
                .GetListAsync(x => x.UserId == userId, pagination.Limit, pagination.Offset, x => x.Notification);
            return new RepositoryPaginationResponse<NotificationResponse>
            {
                Data = _mapper.Map<IEnumerable<NotificationResponse>>(notifications.Data),
                Total = notifications.Total
            };
        }

        public async Task MarkNotificationAsRead(int notificationId, int userId)
        {
            var notification = await _unitOfWork.NotificationUserRepository
                .GetAsync(x => x.NotificationId == notificationId && x.UserId == userId);
            if (notification == null)
                throw new NotFoundException("Notification not exist!");
            notification.IsRead = true;
            _unitOfWork.NotificationUserRepository.Update(notification);
            if (await _unitOfWork.CommitAsync() == 0)
            {
                throw new ConflictException("Mark Notification as read failed!");
            }
        }

        public async Task SendNotificationToListUser(List<int> receiverIds, Notification notification)
        {
            await _unitOfWork.NotificationRepository.AddAsync(notification);
            if (await _unitOfWork.CommitAsync() == 0)
            {
                throw new ConflictException("Add Notification to DB failed!");
            }
            foreach (var receiverId in receiverIds)
            {
                var notificationUser = new NotificationUser()
                {
                    NotificationId = notification.Id,
                    UserId = receiverId
                };
                await _unitOfWork.NotificationUserRepository.AddAsync(notificationUser);
                var toast = _mapper.Map<NotificationResponse>(notificationUser);
                await _hubContext.Clients.Group($"UserID_{receiverId}").ReceiveNotification(toast);
            }
            if (await _unitOfWork.CommitAsync() == 0)
            {
                throw new ConflictException("Add Notification to each user in DB failed!");
            }
        }

        public async Task<int> CountUnReadNotificationByUserId(int userId)
        {
            var notReadNoti = await _unitOfWork.NotificationUserRepository.GetAllAsync(x => x.UserId == userId && x.IsRead == false);
            return notReadNoti.Count();
        }

        public async Task MarkAllNotificationAsRead(int userId)
        {
            var notifications = await _unitOfWork.NotificationUserRepository
                .GetAllAsync(x => x.UserId == userId && !x.IsRead);

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                _unitOfWork.NotificationUserRepository.Update(notification);
            }

            if (await _unitOfWork.CommitAsync() == 0)
            {
                throw new ConflictException("Mark All Notification as read failed!");
            }
        }
    }
}