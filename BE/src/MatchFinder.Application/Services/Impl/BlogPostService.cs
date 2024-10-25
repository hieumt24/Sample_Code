using AutoMapper;
using MatchFinder.Application.Constants;
using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Models.Responses;
using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Exceptions;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Domain.Models;
using MatchFinder.Infrastructure.DataAccess;
using MatchFinder.Infrastructure.Services;

namespace MatchFinder.Application.Services.Impl
{
    public class BlogPostService : IBlogPostService
    {
        private IMapper _mapper;
        private IFileService _fileService;
        private IUnitOfWork _unitOfWork;
        private MatchFinderContext _context;

        public BlogPostService(IFileService fileService, IUnitOfWork unitOfWork, MatchFinderContext context, IMapper mapper)
        {
            _fileService = fileService;
            _unitOfWork = unitOfWork;
            _context = context;
            _mapper = mapper;
        }

        public async Task<BlogPostResponse> CreateAsync(BlogPostCreateRequest request, int UserId)
        {
            if (request.FieldId != null)
            {
                var fieldExist = await _unitOfWork.FieldRepository.GetAsync(x => x.Id == request.FieldId && x.Status == FieldStatus.ACCEPTED);
                if (fieldExist == null)
                {
                    throw new NotFoundException("Field not found");
                }
            }
            var blogPost = new BlogPost
            {
                FieldId = request.FieldId,
                Title = request.Title,
                Content = request.Content,
                IsPinned = request.IsPinned,
                Category = request.Category
            };
            if (request.Thumbnail != null && _fileService.IsImageFile(request.Thumbnail))
                blogPost.ThumbnailUrl = await _fileService.SaveFileAsync(request.Thumbnail);
            var author = await _unitOfWork.UserRepository.GetAsync(x => x.Id == UserId, x => x.Role);
            if (author == null)
            {
                throw new ConflictException("Account invalid!");
            }
            else
            {
                blogPost.AuthorId = UserId;
                if (author.Role.Name == "Admin")
                {
                    blogPost.IsAdmin = true;
                }
            }
            await _unitOfWork.BlogPostRepository.AddAsync(blogPost);
            if (await _unitOfWork.CommitAsync() > 0)
            {
                return _mapper.Map<BlogPostResponse>(blogPost);
            }
            throw new ConflictException("Create blog post fail");
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var blogPost = await _unitOfWork.BlogPostRepository.GetAsync(x => x.Id == id);
            if (blogPost == null)
            {
                throw new NotFoundException("Blog post not found");
            }
            _unitOfWork.BlogPostRepository.SoftDelete(blogPost);
            if (await _unitOfWork.CommitAsync() > 0)
            {
                return true;
            }
            throw new ConflictException("Failed to delete blog post");
        }

        public async Task<RepositoryPaginationResponse<BlogPostResponse>> GetListAsync(BlogPostSearchRequest request)
        {
            var blogPosts = await _unitOfWork.BlogPostRepository
                    .GetListBlogPost(request.FieldId, request.Category, request.IsPinned, request.IsAdmin, request.Limit, request.Offset);
            return new RepositoryPaginationResponse<BlogPostResponse>
            {
                Data = _mapper.Map<IEnumerable<BlogPostResponse>>(blogPosts.Data),
                Total = blogPosts.Total
            };
        }

        public async Task<BlogPostResponse> GetByIdAsync(int id)
        {
            var blogPost = await _unitOfWork.BlogPostRepository.GetAsync(x => x.Id == id);
            if (blogPost == null)
            {
                throw new NotFoundException("Blog post not found");
            }
            return _mapper.Map<BlogPostResponse>(blogPost);
        }
    }
}