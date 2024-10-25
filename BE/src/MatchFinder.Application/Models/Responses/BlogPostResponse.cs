namespace MatchFinder.Application.Models.Responses
{
    public class BlogPostResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Category { get; set; }
        public bool IsPinned { get; set; }
        public string ThumbnailUrl { get; set; }
        public bool IsAdmin { get; set; }
        public int? FieldId { get; set; }
    }
}