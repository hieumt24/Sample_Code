using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Models.Responses;
using MatchFinder.Domain.Models;

namespace MatchFinder.Application.Services
{
    public interface IPartialFieldService
    {
        Task<RepositoryPaginationResponse<PartialFieldResponse>> GetListAsync(PartialFieldFilterRequest filterRequest);

        Task<RepositoryPaginationResponse<PartialFieldResponse>> GetListAvailable(int fid, PartialFieldAvailableRequest filterRequest);

        Task<PartialFieldResponse> GetByIdAsync(int id);

        Task<IEnumerable<PartialFieldWithNumberBookingsResponse>> GetListByFieldAsync(int filedId);

        Task<PartialFieldResponse> CreatePartialFieldAsync(int uid, PartialFieldCreateRequest request);

        Task<PartialFieldResponse> UpdatePartialFieldAsync(int id, PartialFieldUpdateRequest request);

        Task<int> GetOwnerIdAsync(int partialFieldId);
    }
}