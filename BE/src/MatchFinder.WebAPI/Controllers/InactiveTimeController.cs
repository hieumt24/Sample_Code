using MatchFinder.Application.Authorize.Attributes.InactiveTime;
using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Services;
using MatchFinder.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace MatchFinder.WebAPI.Controllers
{
    [Route("api/inactive-times")]
    [ApiController]
    public class InactiveTimeController : BaseApiController
    {
        private readonly IInactiveTimeService _inactiveTimeService;

        public InactiveTimeController(IInactiveTimeService inactiveTimeService)
        {
            _inactiveTimeService = inactiveTimeService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var inactiveTime = await _inactiveTimeService.GetByIdAsync(id);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Get inactive time successfully",
                Data = inactiveTime
            });
        }

        [InactiveTimeAuthorize]
        [HttpPost]
        public async Task<IActionResult> CreateAsync(InactiveTimeCreateRequest request)
        {
            var inactiveTime = await _inactiveTimeService.CreateAsync(request);
            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "Inactive time created successfully",
                Data = inactiveTime
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetListByFieldAsync([FromQuery] InactiveTimeGetRequest request)
        {
            var inactiveTimes = await _inactiveTimeService.GetListByFieldAsync(request);
            return Ok(new PaginationResponse
            {
                Success = true,
                Message = "Get inactive times successfully",
                Data = inactiveTimes.Data,
                Meta = new Meta
                {
                    Limit = request.IsPaging == true ? request.Limit : 0,
                    Offset = request.IsPaging == true ? request.Offset : 0,
                    Total = inactiveTimes.Total
                }
            });
        }

        [InactiveTimeAuthorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, InactiveTimeUpdateRequest request)
        {
            var partialField = await _inactiveTimeService.UpdateAsync(id, request);
            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "Inactive time updated successfully",
                Data = partialField
            });
        }

        [InactiveTimeAuthorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var inactiveTime = await _inactiveTimeService.DeleteAsync(id);
            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "Delete inactive time successfully",
                Data = inactiveTime
            });
        }
    }
}