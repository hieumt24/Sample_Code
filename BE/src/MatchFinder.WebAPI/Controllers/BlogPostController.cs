using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Services;
using MatchFinder.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MatchFinder.WebAPI.Controllers
{
    [Route("api/blog-posts")]
    [ApiController]
    public class BlogPostController : BaseApiController
    {
        private readonly IBlogPostService _blogPostService;

        public BlogPostController(IBlogPostService blogPostService)
        {
            _blogPostService = blogPostService;
        }

        [HttpGet("{blogPostid}")]
        public async Task<IActionResult> GetByBlogPostIdAsync(int blogPostid)
        {
            var blogPost = await _blogPostService.GetByIdAsync(blogPostid);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Get blog post successfully",
                Data = blogPost,
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetListAsync([FromQuery] BlogPostSearchRequest request)
        {
            var blogPosts = await _blogPostService.GetListAsync(request);
            return Ok(new PaginationResponse
            {
                Success = true,
                Message = "Get bookings successfully",
                Data = blogPosts.Data,
                Meta = new Meta
                {
                    Limit = request.Limit,
                    Offset = request.Offset,
                    Total = blogPosts.Total
                }
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] BlogPostCreateRequest request)
        {
            var blogPost = await _blogPostService.CreateAsync(request, UserID);
            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "Create blog post successfully",
                Data = blogPost
            });
        }

        [Authorize]
        [HttpDelete("{blogPostid}")]
        public async Task<IActionResult> DeleteAsync(int blogPostid)
        {
            var result = await _blogPostService.DeleteAsync(blogPostid);
            return Ok(new GeneralBoolResponse
            {
                success = result,
                message = result ? "Delete blog post successfully" : "Delete blog post fail"
            });
        }
    }
}