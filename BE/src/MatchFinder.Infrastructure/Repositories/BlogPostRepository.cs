using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Domain.Models;
using MatchFinder.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace MatchFinder.Infrastructure.Repositories
{
    public class BlogPostRepository : GenericRepository<BlogPost>, IBlogPostRepository
    {
        public BlogPostRepository(MatchFinderContext context) : base(context)
        {
        }

        public async Task<RepositoryPaginationResponse<BlogPost>> GetListBlogPost(int? fieldId, string? category, bool? isPinned, bool? isAdmin, int limit, int offset)
        {
            IQueryable<BlogPost> query = _context.Set<BlogPost>();

            query = query.Where(x =>
                    (!fieldId.HasValue || x.FieldId == fieldId) &&
                    (!isAdmin.HasValue || x.IsAdmin == isAdmin) &&
                    (string.IsNullOrEmpty(category) || x.Category == category) &&
                    (!isPinned.HasValue || x.IsPinned == isPinned) &&
                    x.IsDeleted == false).Select(x => new BlogPost
                    {
                        Id = x.Id,
                        Title = x.Title,
                        AuthorId = x.AuthorId,
                        Category = x.Category,
                        CreatedAt = x.CreatedAt,
                        ThumbnailUrl = x.ThumbnailUrl,
                        LastUpdatedAt = x.LastUpdatedAt,
                        FieldId = x.FieldId,
                        IsPinned = x.IsPinned,
                        IsAdmin = x.IsAdmin,
                    });
            query = query.OrderByDescending(entity => EF.Property<DateTime>(entity, "CreatedAt"));

            var total = await query.CountAsync();
            query = query.Skip(offset).Take(limit);
            var data = await query.ToListAsync();
            return new RepositoryPaginationResponse<BlogPost>
            {
                Data = data,
                Total = total
            };
        }
    }
}