using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Models.Responses;
using MatchFinder.Domain.Models;

namespace MatchFinder.Application.Services
{
    public interface IBlogPostService
    {
        Task<RepositoryPaginationResponse<BlogPostResponse>> GetListAsync(BlogPostSearchRequest request);

        Task<BlogPostResponse> GetByIdAsync(int id);

        Task<bool> DeleteAsync(int id);

        Task<BlogPostResponse> CreateAsync(BlogPostCreateRequest request, int UserId);
    }
}