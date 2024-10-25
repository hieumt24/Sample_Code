using MatchFinder.Application.Services;
using MatchFinder.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static MatchFinder.Application.Models.Requests.UserRequest;

namespace MatchFinder.WebAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : BaseApiController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] UserFilterRequest request)
        {
            var result = await _userService.SearchAsync(request);
            return Ok(new PaginationResponse
            {
                Success = true,
                Message = "Get users successfully",
                Data = result.Data,
                Meta = new Meta
                {
                    Limit = request.Limit,
                    Offset = request.Offset,
                    Total = result.Total
                }
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetailUserFullField(int id)
        {
            var result = await _userService.GetDetailUserFullField(id);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Get user successfully",
                Data = result
            });
        }

        [HttpGet("pub/{id}")]
        public async Task<IActionResult> GetUserByIdAsync(int id)
        {
            var result = await _userService.GetUserByIdAsync(id);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Get user successfully",
                Data = result
            });
        }

        [Authorize]
        [HttpPut("status")]
        public async Task<IActionResult> UpdateStatusUser([FromBody] UserChangeStatusRequest request)
        {
            var result = await _userService.UpdateStatusUser(request);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Update status user successfully",
                Data = result
            });
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserAsync(int id, [FromForm] UserUpdateRequest request)
        {
            var result = await _userService.UpdateUserAsync(id, request);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Update user successfully",
                Data = result
            });
        }
    }
}