using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Models.Responses;
using MatchFinder.Domain.Models;

namespace MatchFinder.Application.Services
{
    public interface IInactiveTimeService
    {
        Task<InactiveTimeResponse> GetByIdAsync(int id);

        Task<RepositoryPaginationResponse<InactiveTimeResponse>> GetListByFieldAsync(InactiveTimeGetRequest request);

        Task<InactiveTimeResponse> CreateAsync(InactiveTimeCreateRequest request);

        Task<InactiveTimeResponse> UpdateAsync(int id, InactiveTimeUpdateRequest request);

        Task<InactiveTimeResponse> DeleteAsync(int id);

        Task CheckDuplicateInactiveTime(int fieldId, DateTime startTime, DateTime endTime);
    }
}