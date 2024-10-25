using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Models.Responses;
using MatchFinder.Domain.Models;

namespace MatchFinder.Application.Services
{
    public interface IFieldService
    {
        Task<RepositoryPaginationResponse<FieldResponse>> GetMyFieldsAsync(int ownerId, GetFieldsRequest request);

        Task<RepositoryPaginationResponse<FieldResponse>> GetStaffFieldsAsync(int sid, GetFieldsRequest request);

        Task<Object> GetFreeSlot(GetFreeSlotRequest request);

        Task<RepositoryPaginationResponse<FieldResponse>> GetFieldsAsync(GetFieldsRequest request);

        Task<object> SearchOptionAsync(SearchOptionRequest request);

        Task<RepositoryPaginationResponse<FieldResponse>> GetAllFieldsAsync(GetFieldsRequest request);

        Task<FieldResponse> GetByIdAsync(int id);

        Task<RepositoryPaginationResponse<FieldResponse>> GetFieldsAsync(FieldsLocationRequest request);

        Task<RepositoryPaginationResponse<FieldResponse>> GetFieldsEarlyAsync(int uid, GetFieldsEarlyRequest request);

        Task<FieldResponse> CreateFieldAsync(FieldCreateRequest request, int id);

        Task<IEnumerable<FieldResponse>> SearchAsync(string keyword);

        Task<RepositoryPaginationResponse<FieldResponse>> GetFieldsByScanRadius(FieldsScanLocationRequest request);

        Task<FieldResponse> RejectFieldAsync(int id);

        Task<FieldResponse> HandleStatusAsync(int id, string status);

        Task SoftDeleteFieldAsync(int id);

        Task<FieldResponse> UpdateFieldAsync(FieldUpdateRequest request);

        Task<FieldResponse> FixedSlotFieldAsync(FixedSlotRequest request);

        Task<Object> CountAllRateAsync(int fid);

        Task<IEnumerable<FieldResponse>> GetRatingField(FieldRatingFilterRequest request);

        Task<IEnumerable<FieldResponse>> GetRecommendField(int fieldId);
    }
}