using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Services;
using MatchFinder.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MatchFinder.WebAPI.Controllers
{
    [Route("api/rates")]
    [ApiController]
    public class RatesController : BaseApiController
    {
        private readonly IRateService _rateService;

        public RatesController(IRateService rateService)
        {
            _rateService = rateService;
        }

        [HttpGet]
        public async Task<IActionResult> GetListByFieldAsync([FromQuery] RateSearchByFieldRequest request)
        {
            var rates = await _rateService.GetListByFieldAsync(request);
            return Ok(new PaginationResponse
            {
                Success = true,
                Message = "Get rates successfully",
                Data = rates.Data,
                Meta = new Meta
                {
                    Limit = request.Limit,
                    Offset = request.Offset,
                    Total = rates.Total
                }
            });
        }

        [Authorize]
        [HttpGet("booking/{bookingId}/rated")]
        public async Task<IActionResult> GetRateAsync(int bookingId)
        {
            var rate = await _rateService.GetRateAsync(UserID, bookingId);

            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "Get Rate successfully",
                Data = rate
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateRatedAsync([FromBody] RateCreateRequest request)
        {
            var rate = await _rateService.CreateRateAsync(UserID, request);

            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "Rate created successfully",
                Data = rate
            });
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateRate([FromBody] RateUpdateRequest request)
        {
            var rate = await _rateService.UpdateRateAsync(UserID, request);

            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "Rate updated successfully",
                Data = rate
            });
        }

        [Authorize]
        [HttpDelete("booking/{bid}")]
        public async Task<IActionResult> DeleteRate(int bid)
        {
            await _rateService.DeleteRateAsync(UserID, bid);

            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "Rate deleted!",
            });
        }
    }
}