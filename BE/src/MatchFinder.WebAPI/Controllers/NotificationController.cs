using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Services;
using MatchFinder.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace MatchFinder.WebAPI.Controllers
{
    [Route("api/notifications")]
    [ApiController]
    public class NotificationController : BaseApiController
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetListAsync([FromQuery] Pagination pagination)
        {
            var notificationResponse = await _notificationService.GetUserNotifications(UserID, pagination);

            return Ok(new PaginationResponse
            {
                Success = true,
                Message = "Get notifications successfully",
                Data = notificationResponse.Data,
                Meta = new Meta
                {
                    Limit = pagination.Limit,
                    Offset = pagination.Offset,
                    Total = notificationResponse.Total
                }
            });
        }

        [HttpGet("unread")]
        public async Task<IActionResult> CountUnReadNotificationAsync()
        {
            var count = await _notificationService.CountUnReadNotificationByUserId(UserID);

            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Get notifications successfully",
                Data = count,
            });
        }

        [HttpPut("{id}/is-read")]
        public async Task<IActionResult> MarkNotificationAsRead(int id)
        {
            await _notificationService.MarkNotificationAsRead(id, UserID);

            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Mark notification as read successfully",
            });
        }

        [HttpPut("mark-all-as-read")]
        public async Task<IActionResult> MarkAllNotificationAsRead()
        {
            await _notificationService.MarkAllNotificationAsRead(UserID);

            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Mark all notification as read successfully",
            });
        }
    }
}