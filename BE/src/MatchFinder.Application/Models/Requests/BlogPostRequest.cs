using Microsoft.AspNetCore.Http;

namespace MatchFinder.Application.Models.Requests
{
    public class BlogPostCreateRequest
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string? Category { get; set; }
        public bool IsPinned { get; set; }
        public IFormFile Thumbnail { get; set; }
        public int? FieldId { get; set; }
    }

    public class BlogPostSearchRequest : Pagination
    {
        public int? FieldId { get; set; }
        public string? Category { get; set; }
        public bool? IsPinned { get; set; }
        public bool? IsAdmin { get; set; }
    }
}