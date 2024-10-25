using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Models.Responses;
using MatchFinder.Domain.Models;

namespace MatchFinder.Application.Services
{
    public interface IFavoriteFieldService
    {
        Task<RepositoryPaginationResponse<FieldResponse>> GetListFavorite(int uid, GetListFavoriteFieldRequest request);

        Task<FieldResponse> AddFavoriteListAsync(int uid, int fid);

        Task DeleteFavoriteAsync(int uid, int fid);
    }
}