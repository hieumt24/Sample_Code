using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Models.Responses;
using MatchFinder.Domain.Models;

namespace MatchFinder.Application.Services
{
    public interface IMenuService
    {
        Task<MenuResponse> CreateMenu(MenuCreateRequest modelRequest);

        Task<MenuResponse> UpdateMenu(int id, MenuUpdateRequest modelRequest);

        Task<RepositoryPaginationResponse<MenuResponse>> GetPagination(MenuFilterRequest filterRequest);

        Task<MenuResponse> GetByIdAsync(int id);
    }
}