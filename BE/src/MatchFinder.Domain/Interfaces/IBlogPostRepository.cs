using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Models;

namespace MatchFinder.Domain.Interfaces
{
    public interface IBlogPostRepository : IGenericRepository<BlogPost>
    {
        Task<RepositoryPaginationResponse<BlogPost>> GetListBlogPost(int? fieldId, string? category, bool? isPinned, bool? isAdmin, int limit, int offset);
    }
}