using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Services;
using MatchFinder.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MatchFinder.WebAPI.Controllers
{
    [Authorize(Roles = "Field")]
    [Route("api/Staffs")]
    [ApiController]
    public class StaffsController : BaseApiController
    {
        private readonly IStaffService _staffService;

        public StaffsController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStaffs([FromQuery] GetStaffsRequest request)
        {
            var staffs = await _staffService.GetStaffsAsync(UserID, request);
            return Ok(new PaginationResponse
            {
                Success = true,
                Message = "Get staffs successfully",
                Data = staffs.Data,
                Meta = new Meta
                {
                    Limit = request.Limit,
                    Offset = request.Offset,
                    Total = staffs.Total
                }
            });
        }

        [HttpGet("{userId}/field/{fieldId}")]
        public async Task<IActionResult> GetStaff(int userId, int fieldId)
        {
            var staff = await _staffService.GetStaffAsync(UserID, userId, fieldId);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Get staff successfully",
                Data = staff
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateStaff([FromBody] CreateStaffRequest request)
        {
            var result = await _staffService.CreateStaffAsync(UserID, request);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Create staff successfully",
                Data = result
            });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStaff([FromBody] UpdateStaffRequest request)
        {
            var result = await _staffService.UpdateStaffAsync(UserID, request);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Create staff successfully",
                Data = result
            });
        }
    }
}