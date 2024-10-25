using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Models.Responses;
using MatchFinder.Domain.Models;

namespace MatchFinder.Application.Services
{
    public interface IStaffService
    {
        Task<RepositoryPaginationResponse<StaffResponse>> GetStaffsAsync(int oid, GetStaffsRequest request);

        Task<StaffResponse> CreateStaffAsync(int oid, CreateStaffRequest request);

        Task<StaffResponse> UpdateStaffAsync(int oid, UpdateStaffRequest request);

        Task<StaffResponse> GetStaffAsync(int oid, int uid, int fid);
    }
}