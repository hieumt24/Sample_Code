using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Models.Responses;

namespace MatchFinder.Application.Services
{
    public interface ITeamService
    {
        Task<TeamResponse> CreateTeamAsync(TeamCreateRequest request, int uid);
    }
}