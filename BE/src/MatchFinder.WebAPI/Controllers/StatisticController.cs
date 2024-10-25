using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Services;
using MatchFinder.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MatchFinder.WebAPI.Controllers
{
    [Route("api/statistics")]
    [ApiController]
    public class StatisticController : BaseApiController
    {
        private readonly IStatisticService _statisticService;

        public StatisticController(IStatisticService statisticService)
        {
            _statisticService = statisticService;
        }

        [HttpGet("booking-monthly")]
        [Authorize]
        public async Task<IActionResult> GetBookingMonthly([FromQuery] StatisticRequestFilter request)
        {
            var result = await _statisticService.GetBookingMonthlyAsync(UserID, request);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Get booking monthly successfully",
                Data = result
            });
        }

        [HttpGet("booking-week-day")]
        [Authorize]
        public async Task<IActionResult> GetBookingWeekDay([FromQuery] StatisticRequestFilter request)
        {
            var result = await _statisticService.GetBookingWeekDayAsync(UserID, request);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Get booking weekday successfully",
                Data = result
            });
        }

        [HttpGet("booking-slots")]
        [Authorize]
        public async Task<IActionResult> GetBookingSlot([FromQuery] StatisticBookingSlotRequest request)
        {
            var result = await _statisticService.GetBookingSlotAsync(UserID, request);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Get booking slots successfully",
                Data = result
            });
        }

        [HttpGet("booking-status")]
        [Authorize]
        public async Task<IActionResult> GetBookingStatus([FromQuery] StatisticRequestFilter request)
        {
            var result = await _statisticService.GetBookingStatusAsync(UserID, request);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Get booking status successfully",
                Data = result
            });
        }

        [HttpGet("users-monthly")]
        [Authorize]
        public async Task<IActionResult> GetRegisterMonthly([FromQuery] StatisticRequestFilter request)
        {
            var result = await _statisticService.GetRegisterMonthlyAsync(UserID, request);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Get register monthly successfully",
                Data = result
            });
        }
    }
}