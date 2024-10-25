using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Services;
using MatchFinder.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace MatchFinder.WebAPI.Controllers
{
    [Route("api/teams")]
    [ApiController]
    public class TeamController : BaseApiController
    {
        private readonly ITeamService _teamService;

        public TeamController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeamAsync(TeamCreateRequest request)
        {
            var teamResponse = await _teamService.CreateTeamAsync(request, UserID);

            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "Team created successfully",
                Data = teamResponse
            });
        }
    }
}