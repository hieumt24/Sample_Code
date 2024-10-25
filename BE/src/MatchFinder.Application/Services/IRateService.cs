using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Models.Responses;
using MatchFinder.Domain.Models;

namespace MatchFinder.Application.Services
{
    public interface IRateService
    {
        Task<RepositoryPaginationResponse<RateResponse>> GetListByFieldAsync(RateSearchByFieldRequest request);

        Task<RateResponse> CreateRateAsync(int uid, RateCreateRequest request);

        Task<RateResponse> UpdateRateAsync(int uid, RateUpdateRequest request);

        Task<RateResponse> GetRateAsync(int uid, int bookingId);

        Task DeleteRateAsync(int uid, int bid);
    }
}